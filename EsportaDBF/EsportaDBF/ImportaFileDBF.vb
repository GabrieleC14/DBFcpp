Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO
Imports DbfDataReader
Imports DotNetDBF
Imports SocialExplorer.IO
Imports SocialExplorer.IO.FastDBF
Imports DbfLibrary
Imports DbfLibrary.Data
Imports System.Text
Imports dBASE.NET
Imports NDbfReader
Imports System.Threading


Public Class ImportaFileDBF

    Dim mPathAttualeLog As String = Path.Combine(Application.StartupPath + "/Log", $"log_{DateTime.Now:yyyy-MM-dd}.txt")

#Region "Funzioni con conversione da DBF a CSV"

    '/////////___________BLOCCO DI FUNZIONI CHE RICHIAMANO UN ESEGUIBILE SCRITTO IN C++ CON CHE IN MODALITA' BINARIA CONVERTE IL FILE DBF IN CSV 

    Public Function ConvertDBFtoCSV(filePath As String) As Boolean

        Dim copiato As Boolean
        Try

            copiato = CopiaFile(filePath)

            If copiato <> True Then

                Dim fileName As String = Path.GetFileNameWithoutExtension(filePath)
                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine("Errore durante la creazione del file CSV " + fileName + " il " + Now.ToString)
                fileLog.Close()
                Return False  'se il file non viene copiato esce dalla funzione e conseguentemente non esporta 
                Exit Function

            End If

            Dim ClientiFornitoriList As New List(Of ClientiFornitori)

            Dim exePath As String = Application.StartupPath + "\DBFCSVcpp\main.exe" 'chiama l'eseguibile che nel main prende in ingresso il path.

            ' Avvia il processo e passa il path come argomento
            Dim p As New Process()
            p.StartInfo.FileName = exePath
            p.StartInfo.Arguments = """" & filePath & """"
            p.StartInfo.UseShellExecute = False
            p.StartInfo.CreateNoWindow = True
            p.Start()

            p.WaitForExit()
            Console.WriteLine("Conversione completata!")

            If p.ExitCode <> 0 Then
                Dim fileName As String = Path.GetFileNameWithoutExtension(filePath)
                Using fileLog As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                    fileLog.WriteLine("Errore: il processo esterno ha restituito ExitCode " & p.ExitCode & " per il file " & fileName & " il " & Now.ToString)
                End Using
                Return False
            End If

        Catch exp As Exception
            Dim fileName As String = Path.GetFileNameWithoutExtension(filePath)
            Dim fileLog As System.IO.StreamWriter
            fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLog.WriteLine("Errore durante la creazione del file CSV " + fileName + " il " + Now.ToString)
            fileLog.Close()
            Return False
        End Try

        Return copiato
    End Function

    Public Function CopiaFile(sourcePath As String) As Boolean

        Try
            Dim fileName As String = Path.GetFileNameWithoutExtension(sourcePath)
            Dim destinationFolder As String = Application.StartupPath + "\Backup\" + fileName

            ' Verifica se il percorso di origine è valido
            If String.IsNullOrWhiteSpace(sourcePath) OrElse Not File.Exists(sourcePath) Then
                Throw New FileNotFoundException("Il file di origine non esiste.", sourcePath)
                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine("Il file di origine" + fileName + " non esiste.")   'Log
                fileLog.Close()
                Return False
                Exit Function
            End If

            ' Verifica se la cartella di destinazione è valida
            If String.IsNullOrWhiteSpace(destinationFolder) OrElse Not Directory.Exists(destinationFolder) Then
                Throw New ArgumentException("La cartella di destinazione non esiste.", NameOf(destinationFolder))
                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine("La cartella di destinazione " + destinationFolder + " non esiste.")   'Log
                fileLog.Close()
                Return False
                Exit Function
            End If

            ' Estrai il nome del file senza estensione
            Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(sourcePath)
            ' Estrai l'estensione del file
            Dim fileExtension As String = Path.GetExtension(sourcePath)
            ' Crea un timestamp valido per i nomi file
            Dim timestamp As String = Now.ToString("yyyyMMdd_HHmmss")
            ' Crea il nuovo nome del file
            Dim newFileName As String = $"{fileNameWithoutExtension}-copia_{timestamp}{fileExtension}"
            ' Percorso di destinazione
            Dim destinationPath As String = Path.Combine(destinationFolder, newFileName)

            ' Copia il file
            File.Copy(sourcePath, destinationPath, overwrite:=True)

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine("il file " + fileName + " è stato copiato il " + Now.ToString)
            fileLo.Close()
            Return True

        Catch ex As Exception
            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(ex.Message + " " + Now.ToString)
            fileLo.Close()
            Return False
        End Try

    End Function


    Public Function LeggiArticoliDaCSV(percorsoCsv As String) As List(Of Articoli)  'Funzione che preleva gli articoli

        Dim articoli As New List(Of Articoli)()

        ' Verifica se il file CSV esiste
        If Not File.Exists(percorsoCsv) Then
            Throw New FileNotFoundException("File CSV non trovato", percorsoCsv)
        End If

        ' Leggi tutte le righe del CSV
        Dim righe = File.ReadAllLines(percorsoCsv)
        If righe.Length = 0 Then Return articoli

        ' Leggi l'intestazione
        Dim intestazioni = righe(0).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

        ' Indici dei campi 
        Dim idxCodice = Array.IndexOf(intestazioni, "COD")
        Dim idxDescr = Array.IndexOf(intestazioni, "DES")
        Dim idxIVA = Array.IndexOf(intestazioni, "COD_IVA")
        Dim idxUM = Array.IndexOf(intestazioni, "UM")
        Dim idxPrezzoAcq = Array.IndexOf(intestazioni, "PRACQU_EU")

        ' Leggi i dati
        For i = 1 To righe.Length - 1

            Dim colonne = righe(i).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

            If i = 18 Then
                Console.Write("!!!")
            End If

            ' Verifica che il numero di colonne sia sufficiente
            If colonne.Length > 0 Then
                Dim a As New Articoli With {
                .CodArticolo = SafeGet(colonne, idxCodice),
                .Descrizione = SafeGet(colonne, idxDescr),
                .IVA = SafeGet(colonne, idxIVA),
                .UM = SafeGet(colonne, idxUM),
                .Prezzo = SafeGet(colonne, idxPrezzoAcq)
            }
                articoli.Add(a)
            End If

        Next


        If idxDescr < 0 Then

            Return Nothing
            Exit Function
        End If

        Return articoli

    End Function

    Public Function LeggiClientiFornitoriDaCSV(percorsoCsv As String) As List(Of ClientiFornitori) 'Funzione che preleva i clienti/fornitori

        Dim clienti As New List(Of ClientiFornitori)()

        ' Verifica se il file CSV esiste
        If Not File.Exists(percorsoCsv) Then
            Throw New FileNotFoundException("File CSV non trovato", percorsoCsv)
        End If

        ' Leggi tutte le righe del CSV
        Dim righe = File.ReadAllLines(percorsoCsv)
        If righe.Length = 0 Then Return clienti

        ' Leggi l'intestazione
        Dim intestazioni = righe(0).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

        ' Indici dei campi 
        Dim idxCodice = Array.IndexOf(intestazioni, "CODICE")
        Dim idxRagSoc = Array.IndexOf(intestazioni, "DEN")
        Dim idxCarattereRiconoscitore = Array.IndexOf(intestazioni, "CL_FR")
        Dim idxIndirizzo = Array.IndexOf(intestazioni, "INDI")
        Dim idxCap = Array.IndexOf(intestazioni, "CAP")
        Dim idxCitta = Array.IndexOf(intestazioni, "CITTA")
        Dim idxProvincia = Array.IndexOf(intestazioni, "PROV")
        Dim idxCodFisc = Array.IndexOf(intestazioni, "CFI")
        Dim idxPIva = Array.IndexOf(intestazioni, "PIVA")
        Dim idxTelefono = Array.IndexOf(intestazioni, "TEL")

        ' Leggi i dati
        For i = 1 To righe.Length - 1

            Dim colonne = righe(i).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

            If i = 18 Then
                Console.Write("!!!")
            End If

            ' Verifica che il numero di colonne sia sufficiente
            If colonne.Length > 0 Then
                Dim c As New ClientiFornitori With {
                .Codice = SafeGet(colonne, idxCodice),
                .RagioneSociale = SafeGet(colonne, idxRagSoc),
                .Indirizzo = SafeGet(colonne, idxIndirizzo),
                .Cap = SafeGet(colonne, idxCap),
                .Citta = SafeGet(colonne, idxCitta),
                .Provincia = SafeGet(colonne, idxProvincia),
                .CodFiscale = SafeGet(colonne, idxCodFisc),
                .PIva = SafeGet(colonne, idxPIva),
                .Telefono = SafeGet(colonne, idxTelefono),
                .CarattereRiconoscitore = SafeGet(colonne, idxCarattereRiconoscitore)
            }
                clienti.Add(c)
            End If
        Next


        If idxRagSoc < 0 Then

            Return Nothing
            Exit Function
        End If

        Return clienti
    End Function

    Public Function LeggiDestinazioniClientiDaCSV(percorsoCsv As String) As List(Of DestinazioniClienti)
        Dim destinazioni As New List(Of DestinazioniClienti)()

        ' Verifica esistenza file
        If Not File.Exists(percorsoCsv) Then
            Dim fileLog As System.IO.StreamWriter
            fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLog.WriteLine("File CSV destinazioni clienti non trovato")
            fileLog.Close()
            Throw New FileNotFoundException("File CSV non trovato", percorsoCsv)
            Exit Function
        End If

        ' Leggi tutte le righe
        Dim righe = File.ReadAllLines(percorsoCsv)
        If righe.Length = 0 Then Return destinazioni

        ' Intestazione
        Dim intestazioni = righe(0).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

        ' Indici colonne
        Dim idxCodCliente = Array.IndexOf(intestazioni, "CODICE")
        Dim idxCodDest = Array.IndexOf(intestazioni, "CODFIL")
        Dim idxIndirizzo = Array.IndexOf(intestazioni, "DESFIL")
        Dim idxCitta = Array.IndexOf(intestazioni, "CITTA")
        Dim idxCap = Array.IndexOf(intestazioni, "CAP")
        Dim idxProvincia = Array.IndexOf(intestazioni, "PR")

        ' Leggi dati
        For i = 1 To righe.Length - 1
            Dim colonne = righe(i).Split(";"c).Select(Function(s) s.Trim("""")).ToArray()

            If colonne.Length > 0 Then
                Dim d As New DestinazioniClienti With {
                .CodiceCliente = SafeGet(colonne, idxCodCliente),
                .CodiceDestinazione = SafeInt(SafeGet(colonne, idxCodDest)),
                .Indirizzo = SafeGet(colonne, idxIndirizzo),
                .Citta = SafeGet(colonne, idxCitta),
                .cap = SafeGet(colonne, idxCap),
                .provincia = SafeGet(colonne, idxProvincia)
            }
                destinazioni.Add(d)
            End If
        Next

        Return destinazioni
    End Function


    Private Function SafeInt(value As String) As Integer 'ritorna zero se non è valido 
        Dim result As Integer
        If Integer.TryParse(value, result) Then
            Return result
        Else
            Return 0
        End If
    End Function


    Private Function SafeGet(colonne As String(), index As Integer) As String
        ' Ritorna una stringa vuota se l'indice non è valido o la colonna non esiste
        If index >= 0 AndAlso index < colonne.Length Then
            Return colonne(index).Trim()
        Else
            Return String.Empty
        End If

    End Function

#End Region

#Region "Funzioni con libreria"

    '/////////////////////////---- BLOCCO DI FUNZIONI CHE ESTRAGGONO TRAMITE LA LIBRERIA DBFDATAREADER (NON SEMPRE FUNZIONANO)

    Public Function EstraiDatiArticoliDBF(PercorsoFile As String) As List(Of Articoli)

        Try
            ' Lista per memorizzare gli articoli
            Dim articoliList As New List(Of Articoli)

            ' DbfDataReader per aprire il file DBF
            Using reader As New DbfDataReader.DbfDataReader(PercorsoFile)

                ' ciclo per estrarre i dati 
                While reader.Read()

                    ' Leggi i dati 
                    Dim articolo As New Articoli With {
                        .CodArticolo = reader("COD").ToString(),   'utilizza il nome che sarà visualizzato a schermo nella visualizzazione del file 
                        .Descrizione = reader("DES").ToString(),
                        .IVA = Convert.ToInt32(reader("IVA")),
                        .UM = reader("UM").ToString(),
                        .Prezzo = Convert.ToDouble(reader("PREZZO_ACQ"))
                    }
                    articoliList.Add(articolo)
                End While
            End Using

            ' Mostra i risultati nella console
            For Each articolo In articoliList
                Console.WriteLine($"{articolo.CodArticolo} - {articolo.Descrizione} - {articolo.IVA} - {articolo.UM} - {articolo.Prezzo}")
            Next

            Return articoliList

        Catch ex As Exception
            Console.Write("Errore in EstraiDatiArticoliDBF() : " + ex.Message)
            Return Nothing
        End Try

    End Function



    Public Function EstraiDatiClientiFornitoriDBF(PercorsoFile As String) As List(Of ClientiFornitori)
        Try
            ' Lista per memorizzare i clienti/fornitori
            Dim ClientiFornitoriList As New List(Of ClientiFornitori)
            Dim counter As Integer
            ' DbfDataReader per aprire il file DBF
            Using reader As New DbfDataReader.DbfDataReader(PercorsoFile)
                ' Dim rd As IDbfValue


                While reader.Read()
                    counter += 1
                    ' Leggi i dati 
                    Dim CliFor As New ClientiFornitori With {
                        .Codice = If(IsNothing(reader("CODICE").ToString), Nothing, reader("CODICE").ToString),  'utilizza il nome che sarà visualizzato a schermo nella visualizzazione del file 
                        .RagioneSociale = If(IsNothing(reader("DEN").ToString), Nothing, reader("DEN").ToString),
                        .Indirizzo = If(IsNothing(reader("INDI").ToString), Nothing, reader("INDI").ToString),
                        .Cap = If(String.IsNullOrEmpty(reader("CAP").ToString()) OrElse Not IsNumeric(reader("CAP").ToString()), Nothing, Convert.ToDecimal(reader("CAP"))),
                        .Citta = If(IsNothing(reader("CITTA").ToString), Nothing, reader("CITTA").ToString),
                        .Provincia = If(IsNothing(reader("PROV").ToString), Nothing, reader("PROV").ToString),
                        .PIva = If(IsNothing(reader("PIVA").ToString), Nothing, reader("PIVA").ToString),
                        .CarattereRiconoscitore = If(IsNothing(reader("CL_FR").ToString), Nothing, reader("CL_FR").ToString),
                        .CodFiscale = If(IsNothing(reader("CFI").ToString), Nothing, reader("CFI").ToString),
                        .Telefono = If(String.IsNullOrEmpty(reader("TEL").ToString()) OrElse Not IsNumeric(reader("TEL").ToString()), Nothing, Convert.ToDecimal(reader("TEL")))
                    }
                    ClientiFornitoriList.Add(CliFor)
                    If counter = 285 Then
                        Console.WriteLine(counter)
                    End If

                End While

                'For Each record In reader.DbfTable.SkipToFirstRecord
                '    Console.WriteLine(record)
                'Next

                'Dim BinaryReader As BinaryReader

                'For i = 0 To reader.FieldCount

                '    Dim CliFor As New ClientiFornitori With {
                '        .Codice = If(IsNothing(reader("CODICE").ToString), Nothing, reader("CODICE").ToString),  'utilizza il nome che sarà visualizzato a schermo nella visualizzazione del file 
                '        .RagioneSociale = If(IsNothing(reader("DEN").ToString), Nothing, reader("DEN").ToString),
                '        .Indirizzo = If(IsNothing(reader("INDI").ToString), Nothing, reader("INDI").ToString),
                '        .Cap = If(String.IsNullOrEmpty(reader("CAP").ToString()) OrElse Not IsNumeric(reader("CAP").ToString()), Nothing, Convert.ToDecimal(reader("CAP"))),
                '        .Citta = If(IsNothing(reader("CITTA").ToString), Nothing, reader("CITTA").ToString),
                '        .Provincia = If(IsNothing(reader("PROV").ToString), Nothing, reader("PROV").ToString),
                '        .PIva = If(IsNothing(reader("PIVA").ToString), Nothing, reader("PIVA").ToString),
                '        .CarattereRiconoscitore = If(IsNothing(reader("CL_FR").ToString), Nothing, reader("CL_FR").ToString),
                '        .CodFiscale = If(IsNothing(reader("CFI").ToString), Nothing, reader("CFI").ToString),
                '        .Telefono = If(String.IsNullOrEmpty(reader("TEL").ToString()) OrElse Not IsNumeric(reader("TEL").ToString()), Nothing, Convert.ToDecimal(reader("TEL")))
                '    }

                '    ClientiFornitoriList.Add(CliFor)

                '    reader.DbfTable.SkipToFirstRecord(BinaryReader)

                'Next

            End Using

            For Each CliFor In ClientiFornitoriList

                Console.WriteLine($"{CliFor.Codice} - {CliFor.RagioneSociale} - {CliFor.Indirizzo} - {CliFor.Cap} - {CliFor.Citta} - {CliFor.Provincia}")

            Next


            Return ClientiFornitoriList

        Catch ex As Exception

            Console.WriteLine("Errore in EstraiDatiClientiFornitoriDBF(): " & ex.Message)
            Return Nothing

        End Try

    End Function


    'Public Function EstraiFatture(PercorsoFile As String) As List(Of Fatture_Gen)

    '    Dim fattureList As New List(Of Fatture_Gen)

    '    Try
    '        ' DbfDataReader per aprire il file DBF
    '        Using reader As New DbfDataReader.DbfDataReader(PercorsoFile)

    '            ' ciclo per estrarre i dati 
    '            While reader.Read()

    '                '  If(IsDBNull(myR.Item("connessione")), False, myR.Item("connessione"))
    '                'If reader("ORDINE") IsN Then

    '                'End If
    '                ' Leggi i dati 
    '                Dim Fattura As New Fatture_Gen With {
    '                            .DataOra = If(IsNothing(Convert.ToDateTime(reader("DATAOR"))), Nothing, Convert.ToDateTime(reader("DATAOR"))),
    '                            .NrOrdine = If(String.IsNullOrEmpty(reader("ORDINE").ToString()) OrElse Not IsNumeric(reader("ORDINE").ToString()), Nothing, Convert.ToInt32(reader("ORDINE"))),
    '                            .Indirizzo = If(IsNothing(reader("INDI").ToString), Nothing, reader("INDI").ToString),
    '                            .Provincia = If(IsNothing(reader("PROV").ToString), Nothing, reader("PROV").ToString),
    '                            .Citta = If(IsNothing(reader("CITTA").ToString), Nothing, reader("CITTA").ToString),
    '                            .Cap = If(String.IsNullOrEmpty(reader("CAP").ToString()) OrElse Not IsNumeric(reader("CAP").ToString()), Nothing, Convert.ToInt32(reader("CAP")))
    '                        }
    '                fattureList.Add(Fattura)

    '            End While

    '        End Using

    '        ' Mostra i risultati nella console
    '        For Each fatture In fattureList
    '            Console.WriteLine($"{fatture.DataOra} - {fatture.Citta} - {fatture.Indirizzo}")
    '        Next

    '        Return fattureList

    '    Catch ex As Exception
    '        Console.Write("Errore in EstraiFatture() : " + ex.Message)
    '        Return fattureList
    '    End Try

    'End Function


    '/////////////////////////---- FINE BLOCCO DI FUNZIONI CHE ESTRAGGONO TRAMITE LA LIBRERIA DBFDATAREADER (NON SEMPRE FUNZIONANO)

#End Region

#Region "Funzioni di test"

    Public Sub exp(PercorsoFile As String)
        Using reader As New DbfDataReader.DbfDataReader(PercorsoFile)

            While reader.Read()
                Console.Write(reader("NAME").ToString)
            End While

        End Using
    End Sub

    'Public Function EstraiDatiClientiFornitoriDBF(PercorsoFile As String) As List(Of ClientiFornitori)
    '    Try

    '        Dim ClientiFornitoriList As New List(Of ClientiFornitori)

    '        Dim reader As New DbfDataReader.DbfDataReader(PercorsoFile)

    '        ' Verifica record nel file
    '        If Not reader.Read() Then
    '            Console.WriteLine("Il file DBF è vuoto o non contiene dati.")
    '            'Return Nothing
    '        End If

    '        For i = 0 To 1000
    '            ' Do

    '            Dim values = reader.DbfRecord.Values

    '            If values.Count >= 11 Then
    '                Dim CliFor As New ClientiFornitori With {
    '                    .Codice = If(values(0) IsNot Nothing, values(0).ToString(), String.Empty),
    '                    .RagioneSociale = If(values(1) IsNot Nothing, values(1).ToString(), String.Empty),
    '                    .Indirizzo = If(values(3) IsNot Nothing, values(3).ToString(), String.Empty),
    '                    .Citta = If(values(5) IsNot Nothing, values(5).ToString(), String.Empty),
    '                    .Provincia = If(values(6) IsNot Nothing, values(6).ToString(), String.Empty),
    '                    .PIva = If(values(8) IsNot Nothing, values(8).ToString(), String.Empty),
    '                    .CarattereRiconoscitore = If(values(2) IsNot Nothing, values(2).ToString(), String.Empty),
    '                    .CodFiscale = If(values(7) IsNot Nothing, values(7).ToString(), String.Empty)
    '                }

    '                '' Gestisci la conversione del CAP in Integer 
    '                'Dim capStr As String = If(values(4) IsNot Nothing, values(4).ToString(), String.Empty)
    '                'If Not Integer.TryParse(capStr, CliFor.Cap) Then
    '                '    ' Se la conversione fallisce, imposta un valore predefinito
    '                '    CliFor.Cap = 0
    '                'End If

    '                '' Gestisci la conversione del telefono in Integer (da stringa a numero)
    '                'Dim telefonoStr As String = If(values(10) IsNot Nothing, values(10).ToString(), String.Empty)
    '                'If Not Integer.TryParse(telefonoStr, CliFor.Telefono) Then
    '                '    ' Se la conversione fallisce, imposta un valore predefinito
    '                '    CliFor.Telefono = 0
    '                'End If

    '                ' Aggiungi l'oggetto alla lista
    '                ClientiFornitoriList.Add(CliFor)
    '            Else
    '                Console.WriteLine("Il numero di campi nel record non corrisponde alle aspettative.")
    '            End If

    '            reader.NextResultAsync()


    '            reader.NextResult()
    '            reader.NextResult()
    '            reader.NextResult()
    '        Next
    '        ' Loop While reader.Read() ' Continua a leggere i record

    '        ' Stampa i risultati (opzionale, per debug)
    '        For Each CliFor In ClientiFornitoriList
    '            Console.WriteLine($"{CliFor.Codice} - {CliFor.RagioneSociale} - {CliFor.Indirizzo} - {CliFor.Cap} - {CliFor.Citta} - {CliFor.Provincia}")
    '        Next

    '        ' Ritorna la lista dei clienti/fornitori
    '        Return ClientiFornitoriList

    '    Catch ex As Exception
    '        Console.WriteLine("Errore in EstraiDatiClientiFornitoriDBF(): " & ex.Message)
    '        Return Nothing
    '    End Try
    'End Function


    '  il file .dbf che stai cercando di leggere (es. cli_forn.dbf) è legato internamente a un database container (DBC), anche se fisicamente non ho quel file .dbc.
    '  Eccezione generata: 'System.Data.OleDb.OleDbException' in System.Data.dll
    '  Errore durante la lettura delle tabelle o della tabella cli_forn: Cannot open file c:\users\pos5\desktop\connettore magtrace\azienda.dbc.


    Public Sub EstraiNomiTabelle(PercorsoFile As String)

        Dim directoryPath As String = Path.GetDirectoryName(PercorsoFile)

        ' Dim connectionString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & directoryPath & ";Extended Properties=dBASE IV;"
        Dim connectionString As String = "Provider=VFPOLEDB.1;Data Source=" & directoryPath & ";Collating Sequence=machine;"


        ' Lista per salvare i nomi delle tabelle
        Dim nomiTabelle As New List(Of String)

        Using connection As New OleDbConnection(connectionString)
            Try
                connection.Open()

                Dim schemaTable As DataTable = connection.GetSchema("Tables")

                For Each row As DataRow In schemaTable.Rows
                    Dim tableName As String = row("TABLE_NAME").ToString()
                    nomiTabelle.Add(tableName)
                Next

                ' Output dei nomi delle tabelle
                If nomiTabelle.Count > 0 Then

                    Console.WriteLine("Tabelle nel file DBF:")
                    For Each nomeTabella In nomiTabelle
                        Console.WriteLine(nomeTabella)
                    Next

                Else
                    Console.WriteLine("Nessuna tabella trovata.")
                End If

                ' Verifica se esiste cli_forn e prova a leggerla
                '  If nomiTabelle.Contains("cli_forn") Then
                Dim query As String = "SELECT CODICE FROM cli_forn"
                Dim adapter As New OleDbDataAdapter(query, connection)
                Dim table As New DataTable()

                adapter.Fill(table)

                Console.WriteLine(vbCrLf & "Contenuto della tabella:")
                For Each row As DataRow In table.Rows
                    For Each col As DataColumn In table.Columns
                        Console.Write(col.ColumnName & ": " & row(col).ToString() & "  ")
                    Next
                    Console.WriteLine()
                Next
                Console.WriteLine()

                '  Else
                ' Console.WriteLine("La tabella 'cli_forn' non è presente nel file.")
                '  End If

            Catch ex As Exception
                Console.WriteLine("Errore durante la lettura delle tabelle o della tabella : " & ex.Message)
            End Try
        End Using
    End Sub



    Public Function EstraiDatiClientiFornitoriDBF2(PercorsoFile As String) As List(Of ClientiFornitori)
        Dim cnn As New System.Data.OleDb.OleDbConnection
        Dim da As New System.Data.OleDb.OleDbDataAdapter
        Dim theDataSet As New DataSet

        cnn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & PercorsoFile & ";Extended Properties=dBASE IV;"

        Try
            cnn.Open()
            da.SelectCommand = New System.Data.OleDb.OleDbCommand("select * from cli_forn", cnn)
            da.Fill(theDataSet)
            'DataGridView1.DataSource = theDataSet
            'DataGridView1.Refresh()
        Catch ex As Exception
            MsgBox("Error while connecting to databse." & vbNewLine & ex.Message & vbNewLine & ex.ToString)
        End Try

    End Function

#End Region

End Class
