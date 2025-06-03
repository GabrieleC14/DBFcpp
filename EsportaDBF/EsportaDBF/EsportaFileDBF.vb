Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO

Public Class EsportaFileDBF

    Dim ImportaFileDBF As New ImportaFileDBF



    Public Function trovaUltimoOrdine(Percorso As String) As Integer

        Dim Ordine As Integer

        ImportaFileDBF.ConvertDBFtoCSV(Percorso)             'converte in csv il file DBF per prendere il numero dell'ordine
        Ordine = GetLastOrdineFromCSV(Percorso + ".csv", 1)  'passo 1 che è l'indice della colonna ordine 
        Ordine += 1                                          'aggiorno l'ordine + 1

        Return Ordine

    End Function

    Public Function trovaUltimoNumeroDocumento(Percorso As String) As Integer

        Dim numeroDocumento As Integer
        numeroDocumento = GetLastOrdineFromCSV(Percorso + ".csv", 41)
        numeroDocumento += 1

        Return numeroDocumento

    End Function

    Public Function EsportaFatturaGen(Ordine As Integer, Percorso As String, Fattura As Fatture_Gen, TotaleSenzaIva As Double, TotaleConIva As Double) As Boolean
        Dim mStrLog As Boolean
        ' Formatta data in formato YYYYMMDD , passo la stringa della data 
        Dim dataOraStr As String = Fattura.DataOra.ToString("yyyyMMdd")
        Dim dataConsegnaStr As String = Fattura.DataConsegna.ToString("yyyyMMdd")
        ' Percorso all'eseguibile C++
        Dim exePath As String = Application.StartupPath + "\DBFIMPFattGencpp\main.exe"
        Dim dataDoc As String = dataOraStr
        Dim magazzino As String = "001"

        If dataConsegnaStr = "00010101" Then
            dataConsegnaStr = dataOraStr
        End If

        ' Argomenti da passare
        Dim args As String = $"""{Percorso}"" {dataOraStr} {Ordine} ""{Fattura.RagioneSociale}"" ""{Fattura.Indirizzo}"" ""{Fattura.Provincia}"" ""{Fattura.Citta}"" ""{Fattura.Cap}"" ""{Fattura.codiceCliente}"" {dataConsegnaStr} {Fattura.NumColli} {TotaleSenzaIva.ToString(System.Globalization.CultureInfo.InvariantCulture)} {TotaleConIva.ToString(System.Globalization.CultureInfo.InvariantCulture)} ""{Fattura.numDoc}"" ""{Fattura.sezionale}"" ""{Fattura.tipo}"" ""{dataDoc}"" ""{magazzino}"""


        ' Avvia il programma
        Dim proc As New Process()
        proc.StartInfo.FileName = exePath
        proc.StartInfo.Arguments = args
        proc.StartInfo.UseShellExecute = False
        proc.StartInfo.CreateNoWindow = True
        proc.Start()
        proc.WaitForExit()

        ' Controlla il codice di uscita, 0 = esportazione riuscita , 1 = esportazione non riuscita
        Dim exitCode As Integer = proc.ExitCode
        If exitCode = 0 Then
            mStrLog = True
        Else
            mStrLog = False
        End If

        Return mStrLog

    End Function

    'C:\_PROGETTI\VBNET\EsportaDBF\EsportaDBF\bin\Debug\DBFIMPcpp\DBFIMPcpp.exe "C:\Users\POS5\Desktop\Connettore MagTrace\Fatture\Demo\fattgen.dbf" "20250522" "12345" "ACME S.p.A." "Via Roma 1" "RM" "Roma" "00100" "CLI789" "20250525" "3"

    Public Function EsportaFatturaRig(ordine As Integer, Percorso As String, Fattura As Fatture_Righe) As Boolean
        Dim mStrLog As Boolean
        Dim exePath As String = Application.StartupPath & "\DBFIMPFattRigcpp\main.exe"

        ' Converte i valori double in formato compatibile con C++ (es. separatore punto)
        Dim qtaStr As String = Fattura.Qta.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim prezzoStr As String = Fattura.PrezzoUnit.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim scontoStr As String = Fattura.Sconto.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim sconto2Str As String = Fattura.Sconto2.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim sconto3Str As String = Fattura.Sconto3.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim ivaStr As String = Fattura.Iva.ToString("F2", CultureInfo.InvariantCulture)
        Dim articolo As String = Fattura.CodiceArticolo

        ' Costruisce gli argomenti in ordine, gestendo spazi e virgolette
        Dim args As String = $"""{Percorso}"" {Fattura.Anno} {ordine} {Fattura.NumRiga} " &
                     $"""{Fattura.CodiceArticolo}"" ""{Fattura.DescrizioneArticolo}"" ""{Fattura.Lotto}"" " &
                     $"{qtaStr} {prezzoStr} {scontoStr} {sconto2Str} {sconto3Str} " &
                     $"""{Fattura.Um}"" {ivaStr} {ivaStr} ""{articolo}"""

        ' Imposta e avvia il processo
        Dim proc As New Process()
        proc.StartInfo.FileName = exePath
        proc.StartInfo.Arguments = args
        proc.StartInfo.UseShellExecute = False
        proc.StartInfo.CreateNoWindow = True
        proc.Start()

        proc.WaitForExit()

        Dim exitCode As Integer = proc.ExitCode
        If exitCode = 0 Then
            mStrLog = True
        Else
            mStrLog = False
        End If
        Return mStrLog
    End Function


    Public Sub EsportaFatturaDiErroreRiga(ordine As Integer, Percorso As String, Fattura As Fatture_Righe) 
        Dim exePath As String = Application.StartupPath & "\DBFIMPFattRigcpp\main.exe"

        ' Converte i valori double in formato compatibile con C++ (es. separatore punto)
        Dim qtaStr As String = Fattura.Qta.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim prezzoStr As String = Fattura.PrezzoUnit.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim scontoStr As String = Fattura.Sconto.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim sconto2Str As String = Fattura.Sconto2.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim sconto3Str As String = Fattura.Sconto3.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Dim ivaStr As String = Fattura.Iva.ToString("F2", CultureInfo.InvariantCulture)


        ' Costruisce gli argomenti in ordine, gestendo spazi e virgolette
        Dim args As String = $"""{Percorso}"" {Fattura.Anno} {ordine} {Fattura.NumRiga} " &
                     $"""*****"" ""Errore durante l'importazione da MagTrace"" """" " &
                     $"0 0 0 0 0 " &
                     $""" "" 0 0"


        ' Imposta e avvia il processo
        Dim proc As New Process()
        proc.StartInfo.FileName = exePath
        proc.StartInfo.Arguments = args
        proc.StartInfo.UseShellExecute = False
        proc.StartInfo.CreateNoWindow = True
        proc.Start()

        proc.WaitForExit()

        Dim exitCode As Integer = proc.ExitCode
        'If exitCode = 0 Then
        '    mStrLog = True
        'Else
        '    mStrLog = False
        'End If
    End Sub

    Function GetLastOrdineFromCSV(csvFilePath As String, ordineColumnIndex As Integer) As Integer

        If Not File.Exists(csvFilePath) Then
            Throw New FileNotFoundException("File CSV non trovato.", csvFilePath)
        End If

        Dim maxOrdine As Integer = 0

        Try

            Using reader As New StreamReader(csvFilePath)
                Dim headerLine As String = reader.ReadLine() ' Legge la prima riga (header) e la scarta

                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    Dim parts() As String = line.Split(";"c)

                    If parts.Length > ordineColumnIndex Then
                        Dim ordineStr As String = parts(ordineColumnIndex).Replace("""", "").Trim()
                        Dim ordineVal As Integer
                        If Integer.TryParse(ordineStr, ordineVal) Then
                            maxOrdine = ordineVal
                        End If
                    End If
                End While
            End Using

        Catch ex As Exception

            Console.Write("Errore: " + ex.Message)

        End Try

        Return maxOrdine
    End Function




    'Public Sub EsportaFattura(Percorso As String)

    '    ' Creo un oggetto Fattura 
    '    Dim Fattura As New Fatture With {
    '        .DataOra = Now,
    '        .Cap = 80046,
    '        .Citta = "Napoli",
    '        .Indirizzo = "Via G.b. Manso",
    '        .NrOrdine = 123,
    '        .Provincia = "NA"
    '    }

    '    Try

    '        ' Connessione al file DBF
    '        Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Percorso & ";Extended Properties=dBASE IV;"
    '        Using conn As New OleDbConnection(connStr)
    '            conn.Open()

    '            ' Comando INSERT (usa i nomi dei campi come nel DBF)
    '            Dim insertCmd As String = "INSERT INTO fattgen (DATAOR, ORDINE, INDI, PROV, CITTA, CAP) VALUES (?, ?, ?, ?, ?, ?)"
    '            Using cmd As New OleDbCommand(insertCmd, conn)
    '                cmd.Parameters.AddWithValue("?", Fattura.DataOra)
    '                cmd.Parameters.AddWithValue("?", Fattura.NrOrdine)
    '                cmd.Parameters.AddWithValue("?", Fattura.Indirizzo)
    '                cmd.Parameters.AddWithValue("?", Fattura.Provincia)
    '                cmd.Parameters.AddWithValue("?", Fattura.Citta)
    '                cmd.Parameters.AddWithValue("?", Fattura.Cap)
    '                cmd.ExecuteNonQuery()
    '            End Using

    '            conn.Close()
    '        End Using

    '    Catch ex As Exception

    '        Console.Write("Errore : " + ex.Message)

    '    End Try


    'End Sub



End Class
