#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <cstring>

#ifdef _WIN32
#include <windows.h>
#define PATH_SEPARATOR '\\'
#else
#define PATH_SEPARATOR '/'
#endif

// Struttura campo DBF
struct DbfFieldDescriptor {
    char name[11];
    char type;
    typedef unsigned char uint8_t;
	typedef unsigned int uint32_t;
    typedef unsigned char decimal;
    char reserved[14];
};

void readDbfFields(const std::string& dbfFile, std::vector<std::string>& fieldNames) {
    std::ifstream dbf(dbfFile.c_str(), std::ios::binary);
    if (!dbf) {
        std::cerr << "Errore: impossibile aprire il file DBF: " << dbfFile << "\n";
        return;
    }

    char header[32];
    dbf.read(header, 32);  // Intestazione DBF

    while (true) {
        char fieldDesc[32];
        dbf.read(fieldDesc, 32);
        if (fieldDesc[0] == 0x0D) break;  // Terminatore descrizione

        std::string fieldName(fieldDesc, fieldDesc + 11);
        size_t end = fieldName.find('\0');
        if (end != std::string::npos) fieldName = fieldName.substr(0, end);

        fieldNames.push_back(fieldName);
    }

    dbf.close();
}

void writeFakeCdx(const std::string& cdxFile, const std::vector<std::string>& fieldNames) {
    std::ofstream cdx(cdxFile.c_str(), std::ios::binary);
    if (!cdx) {
        std::cerr << "Errore: impossibile creare il file CDX: " << cdxFile << "\n";
        return;
    }

    char header[512];
    std::memset(header, 0, sizeof(header));
    header[0] = 0x02;  // Tipo nodo root fittizio
    cdx.write(header, sizeof(header));

    for (size_t i = 0; i < fieldNames.size(); ++i) {
        std::string tag = "TAG_" + fieldNames[i];
        if (tag.length() > 32) tag = tag.substr(0, 32);
        char tagBuffer[32] = {0};
        std::memcpy(tagBuffer, tag.c_str(), tag.length());
        cdx.write(tagBuffer, sizeof(tagBuffer));
    }

    cdx.close();
}

std::string getFilenameOnly(const std::string& path) {
    size_t pos = path.find_last_of("/\\");
    if (pos != std::string::npos)
        return path.substr(pos + 1);
    return path;
}

std::string removeExtension(const std::string& filename) {
    size_t pos = filename.find_last_of('.');
    if (pos != std::string::npos)
        return filename.substr(0, pos);
    return filename;
}

int main() {
    std::string dbfPath, cdxFolder;
    std::cout << "Inserisci il percorso completo del file DBF: ";
    std::getline(std::cin, dbfPath);

    std::cout << "Inserisci la cartella di destinazione del file CDX: ";
    std::getline(std::cin, cdxFolder);

    std::vector<std::string> fieldNames;
    readDbfFields(dbfPath, fieldNames);

    if (fieldNames.empty()) {
        std::cerr << "Nessun campo trovato nel file DBF.\n";
        return 1;
    }

    std::string baseName = removeExtension(getFilenameOnly(dbfPath));
    std::string cdxPath = cdxFolder;
    if (cdxPath[cdxPath.size() - 1] != PATH_SEPARATOR)
        cdxPath += PATH_SEPARATOR;
    cdxPath += baseName + ".cdx";

    std::cout << "Campi trovati:\n";
    for (size_t i = 0; i < fieldNames.size(); ++i) {
        std::cout << " - " << fieldNames[i] << "\n";
    }

    writeFakeCdx(cdxPath, fieldNames);
    std::cout << "File CDX generato in: " << cdxPath << "\n";

    return 0;
}

