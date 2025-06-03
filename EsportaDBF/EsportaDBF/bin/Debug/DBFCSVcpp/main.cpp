#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <iomanip>
#include <windows.h>

struct DBFField { // struct per le intestazioni 
    std::string name;
    char type;
    int length;
    int decimalPlaces;

    DBFField(std::string n, char t, int l, int d)
        : name(n), type(t), length(l), decimalPlaces(d) {}
};

struct DBFRecord {        //struct per i record
    std::vector<std::string> values;
};

std::string readPaddedString(std::ifstream &file, int length) {
    std::string str(length, ' ');
    file.read(&str[0], length);
    return str;
}

std::string convertToUTF8(const std::string& input) {
    int wideLen = MultiByteToWideChar(1252, 0, input.c_str(), -1, NULL, 0);
    if (wideLen == 0) return "";

    std::wstring wideStr(wideLen, 0);
    MultiByteToWideChar(1252, 0, input.c_str(), -1, &wideStr[0], wideLen);

    int utf8Len = WideCharToMultiByte(CP_UTF8, 0, wideStr.c_str(), -1, NULL, 0, NULL, NULL);
    if (utf8Len == 0) return "";

    std::string utf8Str(utf8Len - 1, 0);
    WideCharToMultiByte(CP_UTF8, 0, wideStr.c_str(), -1, &utf8Str[0], utf8Len, NULL, NULL);

    return utf8Str;
}

std::vector<DBFField> readDBFHeader(std::ifstream &file, int &headerLength, int &recordLength) {
    std::vector<DBFField> fields;
    file.seekg(0, std::ios::beg);
    char header[32];
    file.read(header, 32);

    headerLength = static_cast<unsigned char>(header[8]) | (static_cast<unsigned char>(header[9]) << 8);
    recordLength = static_cast<unsigned char>(header[10]) | (static_cast<unsigned char>(header[11]) << 8);

    int numFields = (headerLength - 32 - 1) / 32;
    for (int i = 0; i < numFields; ++i) {
        char fieldBuf[32];
        file.read(fieldBuf, 32);

        std::string name(fieldBuf, 11);
        size_t nullPos = name.find('\0');
        if (nullPos != std::string::npos)
            name = name.substr(0, nullPos);

        char type = fieldBuf[11];
        int length = static_cast<unsigned char>(fieldBuf[16]);
        int decimal = static_cast<unsigned char>(fieldBuf[17]);

        fields.push_back(DBFField(name, type, length, decimal));
    }

    char terminator; //carattere terminatore
    file.read(&terminator, 1);
    return fields;  //ritorna l'intestazione
}

DBFRecord readDBFRecord(std::ifstream &file, const std::vector<DBFField> &fields, int recordLength) {
    DBFRecord record;
    char deleteFlag;

    // Leggi il primo byte (flag di cancellazione)
    file.read(&deleteFlag, 1);

    // Se il flag di cancellazione è '*' o siamo alla fine del file, ignora il record
    if (deleteFlag == '*' || file.eof()) {
        // Leggi e scarta i dati del record cancellato
        file.seekg(recordLength - 1, std::ios::cur); // Spostati alla fine del record ignorato
        return DBFRecord();  // Torna un record vuoto
    }

    // Se non è cancellato, leggi i dati del record
    for (size_t i = 0; i < fields.size(); ++i) {
        std::string value = readPaddedString(file, fields[i].length);
        size_t first = value.find_first_not_of(' ');
        size_t last = value.find_last_not_of(' ');
        if (first != std::string::npos)
            value = value.substr(first, last - first + 1);
        else
            value = "";
        record.values.push_back(value);
    }
    return record;
}

std::string escapeCSV(const std::string& field) {
    std::string escaped = field;
    size_t pos = 0;
    while ((pos = escaped.find('"', pos)) != std::string::npos) {
        escaped.insert(pos, "\"");
        pos += 2;
    }
    return "\"" + escaped + "\"";
}

int main(int argc, char* argv[]) {
    if (argc != 2) {
        std::cerr << "Utilizzo: dbf_to_csv.exe <percorso_file_dbf>" << std::endl;
        return 1;
    }

    std::string dbfFilePath = argv[1];
    std::string csvFilePath = dbfFilePath + ".csv";  //path csv 
 
    std::ifstream dbfFile(dbfFilePath.c_str(), std::ios::binary);
    if (!dbfFile.is_open()) {
        std::cerr << "Errore apertura file DBF: " << dbfFilePath << std::endl;
        return 1;
    }

    std::ofstream csvFile(csvFilePath.c_str(), std::ios::binary);
    if (!csvFile.is_open()) {
        std::cerr << "Errore apertura file CSV: " << csvFilePath << std::endl;
        return 1;
    }

    csvFile << "\xEF\xBB\xBF";  // BOM UTF-8 per Excel

    int headerLength = 0;
    int recordLength = 0;
    std::vector<DBFField> fields = readDBFHeader(dbfFile, headerLength, recordLength);

    // Scrivi l'intestazione CSV
    for (size_t i = 0; i < fields.size(); ++i) {
        csvFile << escapeCSV(fields[i].name);
        if (i < fields.size() - 1)
            csvFile << ";";
    }
    csvFile << "\n";

    dbfFile.seekg(headerLength, std::ios::beg);

    // Leggi e scrivi i record nel file CSV
    while (true) {
        dbfFile.peek();
        if (dbfFile.eof()) break;

        DBFRecord record = readDBFRecord(dbfFile, fields, recordLength);
        if (record.values.empty()) continue;  // Ignora i record cancellati

        for (size_t i = 0; i < record.values.size(); ++i) {
            csvFile << escapeCSV(convertToUTF8(record.values[i]));
            if (i < record.values.size() - 1)
                csvFile << ";";
        }
        csvFile << "\n"; // va a capo
    }

    dbfFile.close(); //chiude i file csv e dbf
    csvFile.close();

    std::cout << "Conversione completata: " << csvFilePath << std::endl;
    return 0;
} 
