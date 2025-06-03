#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <cstdlib>
#include <cstring>

struct Fattura {
    std::string dataOra;
    int nrOrdine;
    std::string ragioneSociale;
    std::string indirizzo;
    std::string provincia;
    std::string citta;
    std::string cap;
    std::string codiceCliente;
    std::string dataConsegna;
    int numColli;
    double totaleE;
    double totale;
    std::string numeroDocumento;
    std::string sezionale;
    std::string tipoDocumento;
    std::string dataDocumento;
    std::string magazzino;
};

struct DBFField {
    std::string name;
    char type;
    int length;
    int decimalPlaces;
};

std::vector<DBFField> readDBFHeader(std::ifstream& file, int& headerLength, int& recordLength) {
    std::vector<DBFField> fields;
    file.seekg(0, std::ios::beg);
    char header[32];
    file.read(header, 32);

    headerLength = (unsigned char)(header[8]) | ((unsigned char)(header[9]) << 8);
    recordLength = (unsigned char)(header[10]) | ((unsigned char)(header[11]) << 8);

    int numFields = (headerLength - 32 - 1) / 32;
    for (int i = 0; i < numFields; ++i) {
        char fieldBuf[32];
        file.read(fieldBuf, 32);

        std::string name(fieldBuf, 11);
        size_t nullPos = name.find('\0');
        if (nullPos != std::string::npos)
            name = name.substr(0, nullPos);

        char type = fieldBuf[11];
        int length = (unsigned char)(fieldBuf[16]);
        int decimal = (unsigned char)(fieldBuf[17]);

        DBFField field;
        field.name = name;
        field.type = type;
        field.length = length;
        field.decimalPlaces = decimal;

        fields.push_back(field);
    }

    return fields;
}

std::string padField(const std::string& value, int length, bool rightAlign) {
    if ((int)value.length() >= length)
        return value.substr(0, length);

    if (rightAlign)
        return std::string(length - value.length(), ' ') + value;
    else
        return value + std::string(length - value.length(), ' ');
}

std::string intToString(int value) {
    char buffer[20];
    std::sprintf(buffer, "%d", value);
    return std::string(buffer);
}

std::string doubleToString(double value, int precision) {
    char buffer[64];
    std::sprintf(buffer, "%.*f", precision, value);
    return std::string(buffer);
}

bool appendRecord(const std::string& dbfFilePath, const Fattura& fattura) {
    std::fstream dbfFile(dbfFilePath.c_str(), std::ios::in | std::ios::out | std::ios::binary);
    if (!dbfFile.is_open()) {
        std::cerr << "Errore apertura file DBF." << std::endl;
        return false;
    }

    int headerLength = 0;
    int recordLength = 0;
    std::ifstream headerFile(dbfFilePath.c_str(), std::ios::binary);
    if (!headerFile.is_open()) {
        std::cerr << "Errore apertura file header DBF." << std::endl;
        dbfFile.close();
        return false;
    }

    std::vector<DBFField> fields = readDBFHeader(headerFile, headerLength, recordLength);
    headerFile.close();

    dbfFile.seekg(-1, std::ios::end);
    char lastByte;
    dbfFile.read(&lastByte, 1);
    if (lastByte == 0x1A)
        dbfFile.seekp(-1, std::ios::end);
    else
        dbfFile.seekp(0, std::ios::end);

    dbfFile.put(0x20); // record attivo

    std::vector<DBFField>::const_iterator it;
    for (it = fields.begin(); it != fields.end(); ++it) {
        std::string val;

        if (it->name == "DATAOR")
            val = padField(fattura.dataOra, it->length, false);
        else if (it->name == "ORDINE")
            val = padField(intToString(fattura.nrOrdine), it->length, true);
        else if (it->name == "DESCRIZ")
            val = padField(fattura.ragioneSociale, it->length, false);
        else if (it->name == "INDI")
            val = padField(fattura.indirizzo, it->length, false);
        else if (it->name == "PROV")
            val = padField(fattura.provincia, it->length, false);
        else if (it->name == "CITTA")
            val = padField(fattura.citta, it->length, false);
        else if (it->name == "CAP")
            val = padField(fattura.cap, it->length, false);
        else if (it->name == "CODICE")
            val = padField(fattura.codiceCliente, it->length, false);
        else if (it->name == "DATAC")
            val = padField(fattura.dataConsegna, it->length, false);
        else if (it->name == "NR_COLL")
            val = padField(intToString(fattura.numColli), it->length, true);
        else if (it->name == "TOTALE_E")
            val = padField(doubleToString(fattura.totaleE, it->decimalPlaces), it->length, true);
        else if (it->name == "TOTALE")
            val = padField(doubleToString(fattura.totale, it->decimalPlaces), it->length, true);
        else if (it->name == "NR_ORDI")
            val = padField(fattura.numeroDocumento, it->length, false);
        else if (it->name == "REGISTR")
            val = padField(fattura.sezionale, it->length, false);
        else if (it->name == "TIPO_DOC")
            val = padField(fattura.tipoDocumento, it->length, false);
        else if (it->name == "DATA_ORDI")
            val = padField(fattura.dataDocumento, it->length, false);
        else if (it->name == "MAGAZ")
             val = padField(fattura.magazzino, it->length, false);
        else
            val = padField("", it->length, false);

        dbfFile.write(val.c_str(), it->length);
    }

    dbfFile.put(0x1A); // ripristina EOF

    dbfFile.seekg(4, std::ios::beg);
    char recBytes[4];
    dbfFile.read(recBytes, 4);
    unsigned int numRecords = *(unsigned int*)recBytes;
    ++numRecords;

    dbfFile.seekp(4, std::ios::beg);
    dbfFile.write(reinterpret_cast<const char*>(&numRecords), 4);

    dbfFile.close();
    return true;
}

int main(int argc, char* argv[]) {
    if (argc < 18) {
        std::cerr << "Uso: programma <path> <dataOra> <ordine> <ragioneSociale> <indirizzo> <provincia> <citta> <cap> <codiceCliente> <dataConsegna> <numColli> <totaleE> <totale> <numeroDocumento> <sezionale> <tipoDocumento> <dataDocumento> <magazzino>" << std::endl;
        return 1;
    }

    Fattura f;
    f.dataOra = argv[2];
    f.nrOrdine = std::atoi(argv[3]);
    f.ragioneSociale = argv[4];
    f.indirizzo = argv[5];
    f.provincia = argv[6];
    f.citta = argv[7];
    f.cap = argv[8];
    f.codiceCliente = argv[9];
    f.dataConsegna = argv[10];
    f.numColli = std::atoi(argv[11]);
    f.totaleE = std::atof(argv[12]);
    f.totale = std::atof(argv[13]);
    f.numeroDocumento = argv[14];
    f.sezionale = argv[15];
    f.tipoDocumento = argv[16];
    f.dataDocumento = argv[17];
    f.magazzino = argv[18];
	

    if (!appendRecord(argv[1], f)) {
        return 1;
    }

    return 0;
}

