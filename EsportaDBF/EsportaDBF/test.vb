Public Class test

    'Public Function EstraiDatiClientiFornitoriDBF(PercorsoFile As String) As List(Of ClientiFornitori)
    '    Try
    '        ' Lista per memorizzare i clienti/fornitori
    '        Dim ClientiFornitoriList As New List(Of ClientiFornitori)

    '        ' DbfDataReader per aprire il file DBF
    '        Dim reader As New DbfDataReader.DbfDataReader(PercorsoFile)

    '        Dim rd As IDbfValue



    '        ' Ciclo per estrarre i dati
    '        While reader.Read()
    '            ' Leggi i dati 
    '            Dim CliFor As New ClientiFornitori With {
    '                .Codice = reader("CODICE").ToString(),   'utilizza il nome che sarà visualizzato a schermo nella visualizzazione del file 
    '                .RagioneSociale = reader("DEN").ToString(),
    '                .Indirizzo = reader("INDI").ToString(),
    '                .Cap = reader("CAP").ToString(),
    '                .Citta = reader("CITTA").ToString(),
    '                .Provincia = reader("PROV").ToString,
    '                .PIva = reader("PIVA").ToString,
    '                .CarattereRiconoscitore = reader("CL_FR").ToString,
    '                .CodFiscale = reader("CFI").ToString,
    '                                    .Telefono = Convert.ToInt32(reader("TEL"))
    '            }
    '            ClientiFornitoriList.Add(CliFor)

    '        End While

    '        Dim i = 0

    '        For Each read In reader

    '            Dim CliFor As New ClientiFornitori With {
    '                .Codice = read(i).ToString,
    '                 .RagioneSociale = reader(i).ToString,
    '                        .Indirizzo = reader(i).ToString,
    '                        .Cap = Convert.ToInt32(reader(i)),
    '                        .Citta = reader(i).ToString,
    '                        .Provincia = reader(i).ToString,
    '                        .PIva = reader(i).ToString,
    '                        .CarattereRiconoscitore = reader(i).ToString,
    '                        .CodFiscale = reader(i).ToString,
    '                        .Telefono = Convert.ToInt32(reader(i).ToString)
    '                    }
    '            ClientiFornitoriList.Add(CliFor)

    '            reader.NextResult()
    '            i = i + 1
    '        Next


    '        For Each CliFor In ClientiFornitoriList
    '            Console.WriteLine($"{CliFor.Codice} - {CliFor.RagioneSociale} - {CliFor.Indirizzo} - {CliFor.Cap} - {CliFor.Citta} - {CliFor.Provincia}")
    '        Next

    '        Return ClientiFornitoriList
    '    Catch ex As Exception
    '        Console.WriteLine("Errore in EstraiDatiClientiFornitoriDBF(): " & ex.Message)
    '        Return Nothing
    '    End Try
    'End Function




    'For i = 0 To 100000


    '    Dim CliFor As New ClientiFornitori With {
    '               .Codice = reader.DbfRecord.Values(0).ToString,
    '                .RagioneSociale = reader.DbfRecord.Values(1).ToString,
    '                .Indirizzo = reader.DbfRecord.Values(3).ToString,
    '                .Cap = Convert.ToInt32(reader.DbfRecord.Values(4)),
    '                .Citta = reader.DbfRecord.Values(5).ToString,
    '                .Provincia = reader.DbfRecord.Values(6).ToString,
    '                .PIva = reader.DbfRecord.Values(8).ToString,
    '                .CarattereRiconoscitore = reader.DbfRecord.Values(2).ToString,
    '                .CodFiscale = reader.DbfRecord.Values(7).ToString,
    '                .Telefono = Convert.ToInt32(reader.DbfRecord.Values(10))
    '                }
    '    ClientiFornitoriList.Add(CliFor)

    'Next

    'End Using

    ' Mostra i risultati nella console













    'Public Function EstraiDatiDaDBF(PercorsoFile As String) 'As List(Of String)
    '    Dim risultati As New List(Of String)
    '    ' Usa Microsoft.ACE.OLEDB.12.0 per la connessione DBF
    '    Dim connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & PercorsoFile & ";Extended Properties=dBASE IV;"

    '    ' Verifica che il file esista
    '    If Not IO.File.Exists(PercorsoFile) Then
    '        MessageBox.Show("File DBF non trovato!")
    '        Return risultati
    '    End If

    '    Try
    '        ' Connessione con il file DBF tramite OleDb
    '        Using conn As New OleDbConnection(connectionString)
    '            conn.Open()
    '            ' Ottieni il nome del file DBF senza estensione
    '            Dim dbfName As String = IO.Path.GetFileName(PercorsoFile)
    '            Dim query As String = "SELECT * FROM [" & dbfName & "]"

    '            ' Esegui la query
    '            Using cmd As New OleDbCommand(query, conn)
    '                Using reader As OleDbDataReader = cmd.ExecuteReader()
    '                    ' Leggi ogni record
    '                    While reader.Read()
    '                        Dim riga As String = ""
    '                        For i As Integer = 0 To reader.FieldCount - 1
    '                            riga &= $"{reader.GetName(i)}={reader(i).ToString()} | "
    '                        Next
    '                        risultati.Add(riga)
    '                    End While
    '                End Using
    '            End Using
    '        End Using
    '    Catch ex As Exception
    '        Console.Write("Errore nella lettura del file DBF: " & ex.Message)
    '    End Try

    '    Return risultati
    'End Function

    'Public Sub CaricaFileDBF(PercorsoFile As String)
    '    ' Crea una lista di dizionari per contenere i dati
    '    Dim risultati As New List(Of Dictionary(Of String, Object))

    '    Try
    '        ' Apri il file DBF con DBFReader
    '        Using dbfReader As New DBFReader(PercorsoFile)
    '            ' Leggi i dati del file
    '            While dbfReader.Read()
    '                Dim record As New Dictionary(Of String, Object)

    '                ' Itera attraverso i campi del record
    '                For i As Integer = 0 To dbfReader.FieldCount - 1
    '                    ' Aggiungi il nome del campo e il valore al dizionario
    '                    Dim nomeCampo As String = dbfReader.Fields(i).Name
    '                    Dim valoreCampo As Object = dbfReader.Fields(i).Value
    '                    record.Add(nomeCampo, valoreCampo)
    '                Next

    '                ' Aggiungi il record alla lista dei risultati
    '                risultati.Add(record)
    '            End While
    '        End Using

    '        ' Visualizza i dati nella DataGridView
    '        VisualizzaDati(risultati)
    '    Catch ex As Exception
    '        MessageBox.Show("Errore nella lettura del file DBF: " & ex.Message)
    '    End Try
    'End Sub

    'Public Function EstraiDatiDaDBF(PercorsoFile As String) As List(Of String)
    '    Dim risultati As New List(Of String)

    '    Try
    '        Using dbfReader As New DBFReader(PercorsoFile)
    '            ' Ottieni tutti i record
    '            Dim records As List(Of Dictionary(Of String, Object)) = dbfReader.RecordCount()

    '            For Each record As Dictionary(Of String, Object) In records
    '                Dim riga As String = ""
    '                For Each campo As KeyValuePair(Of String, Object) In record
    '                    riga &= $"{campo.Key}={campo.Value.ToString()} | "
    '                Next
    '                risultati.Add(riga)
    '            Next
    '        End Using
    '    Catch ex As Exception
    '        MessageBox.Show("Errore nella lettura del file DBF: " & ex.Message)
    '    End Try

    '    Return risultati
    'End Function


    '    While reader.Read()
    '    ' Leggi i dati 
    '    Dim CliFor As New ClientiFornitori With {
    '    .Codice = reader("CODICE").ToString(),   'utilizza il nome che sarà visualizzato a schermo nella visualizzazione del file 
    '    .RagioneSociale = reader("DEN").ToString(),
    '    .Indirizzo = reader("INDI").ToString(),
    '    .Cap = reader("CAP").ToString(),
    '    .Citta = reader("CITTA").ToString(),
    '    .Provincia = reader("PROV").ToString,
    '    .PIva = reader("PIVA").ToString,
    '    .CarattereRiconoscitore = reader("CL_FR").ToString,
    '    .CodFiscale = reader("CFI").ToString,
    '    .Telefono = Convert.ToInt32(reader("TEL"))
    '}
    '                    ClientiFornitoriList.Add(CliFor)

    '                End While


    ''Public Sub estrai(PercorsoFile As String)

    ''    Dim articoliList As New List(Of Articoli)

    ''    ' Ottieni la cartella e il nome file senza estensione
    ''    Dim cartellaDBF As String = Path.GetDirectoryName(PercorsoFile)
    ''    Dim nomeTabella As String = Path.GetFileNameWithoutExtension(PercorsoFile)

    ''    ' Usa solo la cartella nella stringa di connessione (non serve Extended Properties)
    ''    Dim connectionString As String = "Provider=VFPOLEDB.1;Data Source=" & cartellaDBF & ";Exclusive=Yes;"

    ''    Using connection As New OleDbConnection(connectionString)
    ''        Try
    ''            connection.Open()

    ''            ' Query sul nome del file senza estensione
    ''            Dim query As String = $"SELECT COD, DES, IVA, UM, PREZZO_ACQ FROM {nomeTabella}"
    ''            Dim command As New OleDbCommand(query, connection)

    ''            Using reader As OleDbDataReader = command.ExecuteReader()
    ''                While reader.Read()
    ''                    Dim articolo As New Articoli With {
    ''                    .CodArticolo = reader("COD").ToString(),
    ''                    .Descrizione = reader("DES").ToString(),
    ''                    .IVA = Convert.ToInt32(reader("IVA")),
    ''                    .UM = reader("UM").ToString(),
    ''                    .Prezzo = Convert.ToDouble(reader("PREZZO_ACQ"))
    ''                }
    ''                    articoliList.Add(articolo)
    ''                End While
    ''            End Using

    ''        Catch ex As Exception
    ''            Console.WriteLine("Errore durante la lettura del file DBF: " & ex.Message)
    ''        End Try
    ''    End Using

    ''    ' Mostra i dati letti dalla lista 
    ''    For Each articolo In articoliList
    ''        Console.WriteLine($"{articolo.CodArticolo} - {articolo.Descrizione} - {articolo.IVA} - {articolo.UM} - {articolo.Prezzo}")
    ''    Next

    ''End Sub


    'Public Sub EstraiFreeTable(ByVal PercorsoFile As String)

    '    Dim articoliList As New List(Of Articoli)

    '    ' Ottieni cartella e nome tabella senza estensione
    '    Dim cartellaDBF As String = Path.GetDirectoryName(PercorsoFile)
    '    Dim nomeTabella As String = Path.GetFileNameWithoutExtension(PercorsoFile)

    '    ' Connessione VFPOLEDB senza DBC
    '    Dim connectionString As String =
    '        $"Provider=VFPOLEDB.1;Data Source={cartellaDBF};Collating Sequence=general;"

    '    Using connection As New OleDbConnection(connectionString)
    '        Try
    '            connection.Open()

    '            ' Disabilita EXCLUSIVE per permettere lettura anche se il file è aperto da altri
    '            Dim disableExclusiveCmd As New OleDbCommand("SET EXCLUSIVE OFF", connection)
    '            disableExclusiveCmd.ExecuteNonQuery()

    '            ' Forza l'uso come Free Table con [] o ``
    '            Dim query As String = $"SELECT COD, DES, IVA, UM, PREZZO_ACQ FROM [{nomeTabella}]"
    '            Dim command As New OleDbCommand(query, connection)

    '            Using reader As OleDbDataReader = command.ExecuteReader()
    '                While reader.Read()
    '                    Dim articolo As New Articoli With {
    '                        .CodArticolo = reader("COD").ToString(),
    '                        .Descrizione = reader("DES").ToString(),
    '                        .IVA = Convert.ToInt32(reader("IVA")),
    '                        .UM = reader("UM").ToString(),
    '                        .Prezzo = Convert.ToDouble(reader("PREZZO_ACQ"))
    '                    }
    '                    articoliList.Add(articolo)
    '                End While
    '            End Using

    '        Catch ex As Exception
    '            Console.WriteLine(" Errore durante la lettura del file DBF come Free Table: " & ex.Message)
    '        End Try
    '    End Using

    '    ' Stampa i risultati
    '    If articoliList.Count > 0 Then
    '        Console.WriteLine(" Articoli estratti:")
    '        For Each articolo In articoliList
    '            Console.WriteLine($"{articolo.CodArticolo} - {articolo.Descrizione} - {articolo.IVA} - {articolo.UM} - {articolo.Prezzo}")
    '        Next
    '    Else
    '        Console.WriteLine(" Nessun articolo trovato.")
    '    End If
    'End Sub

End Class
