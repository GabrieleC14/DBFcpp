#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <cstdlib>

struct FatturaRiga { //struct della riga fattura.
    int anno;
    int ordine;
    int numRiga;
    std::string codiceArticolo;
    std::string descrizioneArticolo;
    std::string lotto;
    double qta;
    double prezzoUnit;
    double sconto;
    double sconto2;
    double sconto3;
    std::string um;
    double iva;
    double aliq;
    std::string articolo;
};

struct DBFField {     //struct per l'intestazione
    std::string name;
    char type;
    int length;
    int decimalPlaces;
};

std::vector<DBFField> readDBFHeader(std::ifstream& file, int& headerLength, int& recordLength) {   // legge l'intestazione 
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
        int length = (unsigned char)fieldBuf[16];
        int decimal = (unsigned char)fieldBuf[17];

        DBFField field;
        field.name = name;
        field.type = type;
        field.length = length;
        field.decimalPlaces = decimal;

        std::cout << "Campo letto: " << field.name << " (len=" << length << ", type=" << type << ")" << std::endl;  //PER IL DEBUG (controllo il campo letto)
        fields.push_back(field);
    }

    return fields;
}

std::string padField(const std::string& value, int length, bool rightAlign) { //sistema le stringhe aggiungendo spazi
    if ((int)value.length() >= length)
        return value.substr(0, length);
    if (rightAlign)
        return std::string(length - value.length(), ' ') + value;
    return value + std::string(length - value.length(), ' ');
}

std::string doubleToString(double value, int precision) { //converte da double a string
    char buffer[64];
    sprintf(buffer, "%.*f", precision, value);
    return std::string(buffer);
}

std::string intToString(int value) { //converte da int a string
    char buffer[20];
    sprintf(buffer, "%d", value);
    return std::string(buffer);
}

bool appendRecord(const std::string& dbfFilePath, const FatturaRiga& riga) {  //aggiunge il record in basso
    std::fstream dbfFile(dbfFilePath.c_str(), std::ios::in | std::ios::out | std::ios::binary);
    if (!dbfFile.is_open()) {
    	
        std::cerr << "Errore apertura file DBF." << std::endl;
        return false;
        
    }

    int headerLength, recordLength;
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

    for (std::vector<DBFField>::const_iterator it = fields.begin(); it != fields.end(); ++it) {  //scorre la lista delle intestazioni 
        std::string val;

        if (it->name == "ANNO")
            val = padField(intToString(riga.anno), it->length, true);
        else if (it->name == "ORDINE")
            val = padField(intToString(riga.ordine), it->length, true);
        else if (it->name == "NUMRIGA")
            val = padField(intToString(riga.numRiga), it->length, true);
        else if (it->name == "COD")
            val = padField(riga.codiceArticolo, it->length, false);
        else if (it->name == "DES_ARTI")
            val = padField(riga.descrizioneArticolo, it->length, false);
        else if (it->name == "LOTTO")
            val = padField(riga.lotto, it->length, false);
        else if (it->name == "QUANTITA")
            val = padField(doubleToString(riga.qta, it->decimalPlaces), it->length, true);
        else if (it->name == "PR_UNIT")
            val = padField(doubleToString(riga.prezzoUnit, it->decimalPlaces), it->length, true);
        else if (it->name == "SCONTO")
            val = padField(doubleToString(riga.sconto, it->decimalPlaces), it->length, true);
        else if (it->name == "SCONTO2")
            val = padField(doubleToString(riga.sconto2, it->decimalPlaces), it->length, true);
        else if (it->name == "SCONTO3")
            val = padField(doubleToString(riga.sconto3, it->decimalPlaces), it->length, true);
        else if (it->name == "UNITA_MISU")
            val = padField(riga.um, it->length, false);
        else if (it->name == "IVA")
            val = padField(doubleToString(riga.iva, it->decimalPlaces), it->length, true);
        else if (it->name == "ALIQ")
            val = padField(doubleToString(riga.aliq, it->decimalPlaces), it->length, true);
        else if (it->name == "ARTICOLO")
            val = padField(riga.articolo, it->length, false);
        else
            val = padField("", it->length, false);

        std::cout << "Scrivo campo [" << it->name << "]: '" << val << "'" << std::endl;
        dbfFile.write(val.c_str(), it->length);
    }

    dbfFile.put(0x1A); // fine file

    dbfFile.seekg(4, std::ios::beg);
    char recBytes[4];
    dbfFile.read(recBytes, 4);
    unsigned int numRecords = *(unsigned int*)recBytes;
    ++numRecords;
    dbfFile.seekp(4, std::ios::beg);
    dbfFile.write(reinterpret_cast<const char*>(&numRecords), 4);

    dbfFile.close(); //chiude il file 
    std::cout << "Record aggiunto con successo." << std::endl; 
    return true;
}

int main(int argc, char* argv[]) {
    if (argc < 16) {
        std::cerr << "Uso: programma <path> <anno> <ordine> <riga> <codArt> <descArt> <lotto> <qta> <prezzo> <sconto> <sconto2> <sconto3> <um> <iva> <aliq> <articolo>" << std::endl;
        return 1;
    }

    FatturaRiga r; //creo l'oggetto riga
    r.anno = atoi(argv[2]);
    r.ordine = atoi(argv[3]);
    r.numRiga = atoi(argv[4]);
    r.codiceArticolo = argv[5];
    r.descrizioneArticolo = argv[6];
    r.lotto = argv[7];
    r.qta = atof(argv[8]);
    r.prezzoUnit = atof(argv[9]);
    r.sconto = atof(argv[10]);
    r.sconto2 = atof(argv[11]);
    r.sconto3 = atof(argv[12]);
    r.um = argv[13];
    r.iva = atof(argv[14]);
    r.aliq = atof(argv[15]);
	r.articolo = argv[16];
	
    if (!appendRecord(argv[1], r)) {
        return 1; //non riuscito
    }

    return 0;  //insert andata a buon fine
}

