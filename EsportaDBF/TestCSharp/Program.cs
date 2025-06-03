using System;
using System.Collections.Generic;
using System.IO;
using DbfDataReader;

class Program
{
    static void test(string dbfFilePath, string csvFilePath)
    {
        // Esegui la conversione da DBF a CSV
        ConvertDbfToCsv(dbfFilePath,  csvFilePath);
    }

    // Funzione per convertire DBF in CSV
    public static void ConvertDbfToCsv(string dbfFilePath, string csvFilePath)
    {
        // Apre il file DBF come stream di lettura
        try
        {
            using (var dbfStream = new FileStream(dbfFilePath, FileMode.Open, FileAccess.Read))
            {
                // Usa DbfDataReader per leggere il DBF
                using (var dbfReader = new DbfDataReader.DbfDataReader(dbfFilePath))
                {
                    // Crea un flusso di scrittura per il file CSV
                    using (var writer = new StreamWriter(csvFilePath))
                    {
                        // Scrive l'intestazione del CSV
                        WriteCsvHeader(dbfReader, writer);

                        // Scrive i dati nel CSV
                        WriteCsvData(dbfReader, writer);
                    }
                }
            }
            Console.WriteLine($"Conversione completata con successo. File CSV creato in: {csvFilePath}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Errore: Il file DBF '{dbfFilePath}' non è stato trovato.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Errore di I/O durante la lettura del file DBF o la scrittura del file CSV: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Si è verificato un errore inaspettato: {ex.Message}");
        }
    }

    // Funzione per scrivere l'intestazione del CSV
    private static void WriteCsvHeader(DbfDataReader.DbfDataReader dbfReader, StreamWriter writer)
    {
        List<string> fieldNames = new List<string>();

        // Verifica se la proprietà FieldCount esiste
        if (dbfReader.GetType().GetProperty("FieldCount") != null)
        {
            int fieldCount = (int)dbfReader.GetType().GetProperty("FieldCount")?.GetValue(dbfReader, null);

            // Prova a ottenere i nomi dei campi tramite indice
            for (int i = 0; i < fieldCount; i++)
            {
                // Verifica se il metodo GetName(i) esiste
                var getNameMethod = dbfReader.GetType().GetMethod("GetName", new Type[] { typeof(int) });
                if (getNameMethod != null)
                {
                    try
                    {
                        string fieldName = getNameMethod.Invoke(dbfReader, new object[] { i })?.ToString();
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            fieldNames.Add(fieldName);
                        }
                        else
                        {
                            fieldNames.Add($"Colonna{i + 1}"); // Fallback se il nome è nullo o vuoto
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore durante la lettura del nome del campo all'indice {i}: {ex.Message}");
                        fieldNames.Add($"ErroreColonna{i + 1}"); // Aggiungi un nome di errore come fallback
                    }
                }
                else
                {
                    fieldNames.Add($"Colonna{i + 1}"); // Fallback se GetName non esiste
                }
            }

            // Scrive l'intestazione nel CSV
            writer.WriteLine(string.Join(",", fieldNames));
        }
        else
        {
            Console.WriteLine("Avviso: Impossibile determinare il numero di campi.");
            writer.WriteLine("Colonna1"); // Scrive almeno un'intestazione per evitare errori a valle
        }
    }

    // Funzione per scrivere i dati nel CSV
    private static void WriteCsvData(DbfDataReader.DbfDataReader dbfReader, StreamWriter writer)
    {
        // Legge ogni record (riga) dal file DBF
        while (dbfReader.Read())
        {
            List<string> values = new List<string>();

            // Aggiungi ogni valore di campo alla lista
            for (int i = 0; i < dbfReader.FieldCount; i++)
            {
                values.Add(dbfReader[i]?.ToString()); // Gestisci valori nulli
            }

            // Scrive la riga dei dati nel CSV
            writer.WriteLine(string.Join(",", values));
        }
    }
}
