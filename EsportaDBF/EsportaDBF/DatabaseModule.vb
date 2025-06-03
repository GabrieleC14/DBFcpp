Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography.X509Certificates
Imports System.Text

Public Interface DatabaseModule
    Function InserisciArticolo(articolo As Articoli, connectionString As String) As Boolean
    Function InserisciClientiFornitori(clienteFornitore As ClientiFornitori, connectionString As String) As Boolean
    Function ArticoloGiaPresente(CodArticolo As String, connectionString As String) As Boolean
    Function UpdateArticolo(articolo As Articoli, connectionString As String)
    Function ClienteFornitoreGiaPresente(codice As String, CarattereRiconoscitore As String, connectionString As String) As Boolean
    Function UpdateClienteFornitore(clienteFornitore As ClientiFornitori, connectionString As String)
End Interface

Public Class ModelFunction
    Implements DatabaseModule
    Dim mPathAttualeLog As String = Path.Combine(Application.StartupPath + "/Log", $"log_{DateTime.Now:yyyy-MM-dd}.txt")
    Dim mMyC As SqlConnection
    Dim mMCm As SqlCommand
    Dim mMyR As SqlDataReader

    'Public Sub New()
    'End Sub

    '//////////_____________ blocco per l'inserimento degli articoli

    Public Function InserisciArticolo(articolo As Articoli, connectionString As String) As Boolean Implements DatabaseModule.InserisciArticolo

        Dim um As Integer = trovaUM(articolo.UM, connectionString)  'trova il keyId Anag_UM
        Dim prezzo As Double
        Dim iva As Integer
        Dim dblIva As Double
        Dim codice As String
        Dim descrizione As String

        If um = Nothing Then
            um = 1
        End If

        If Double.TryParse(articolo.Prezzo.Replace(".", ","), prezzo) Then
            Console.WriteLine("Conversione prezzo riuscita")
        Else
            prezzo = 0
        End If


        If Double.TryParse(articolo.IVA, dblIva) Then

            iva = trovaIVA(dblIva, connectionString)

            If iva = Nothing Then
                iva = 1
            End If

            Console.WriteLine("Conversione prezzo riuscita")
        Else
            iva = 1
        End If


        'If articolo.Descrizione.Contains("�") Then                      'test caratteri speciali
        '    '    CorreggiCaratteriErrati(articolo.Descrizione)
        '    Console.WriteLine(articolo.Descrizione)
        'End If

        'codice = articolo.CodArticolo.Replace("�", "ù")
        'descrizione = articolo.Descrizione.Replace("�", "ù")

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand
            mMCm.CommandText = "INSERT INTO Anag_Articoli(KeyIdAnag_Reparti, KeyIdAnag_Gm, strCodice, strDescrizione, KeyIdAnag_Um, dblPrezzo, strCodiceStatistico, bitVariato, bitEliminato, keyIdAnag_Iva) Values (1 , 1 , @CodArticolo , @Descrizione , @UM , @Prezzo, 1, 0, 0, @IVA) "
            'Anag_Articoli , Anag_ArticoliDatiAggiuntivi

            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@CodArticolo", articolo.CodArticolo)
            mMCm.Parameters.AddWithValue("@Descrizione", articolo.Descrizione)
            mMCm.Parameters.AddWithValue("@IVA", iva)
            mMCm.Parameters.AddWithValue("@UM", um)
            mMCm.Parameters.AddWithValue("@Prezzo", prezzo)

            Try
                mMyC.Open()

                mMCm.ExecuteNonQuery()
                mMyC.Close()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

            Return True
            '  Console.WriteLine("Articolo aggiunto")

        Catch Ex As Exception

            MessageBox.Show(Ex.Message)
            Return False

        End Try

    End Function

    Public Function articoloGiaPresente(CodArticolo As String, connectionString As String) As Boolean

        Dim esistente As Boolean = False

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT strCodice FROM Anag_Articoli WHERE strCodice = @CodArticolo"

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@CodArticolo", CodArticolo)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                If If(IsDBNull(mMyR.Item("strCodice").ToString()), Nothing, mMyR.Item("strCodice").ToString()) <> Nothing Then
                    esistente = True
                Else
                    esistente = False
                End If
            Loop

            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False
        End Try

        Return esistente

    End Function

    Public Function UpdateArticolo(articolo As Articoli, connectionString As String)

        Dim um As Integer = trovaUM(articolo.UM, connectionString)  'trova il keyId Anag_UM
        Dim prezzo As Double
        Dim iva As Integer
        Dim dblIva As Double
        Dim codice As String
        Dim descrizione As String

        If um = Nothing Then
            um = 1
        End If

        If Double.TryParse(articolo.Prezzo.Replace(".", ","), prezzo) Then
            Console.WriteLine("Conversione prezzo riuscita")
        Else
            prezzo = 0
        End If

        If Double.TryParse(articolo.IVA, dblIva) Then

            iva = trovaIVA(dblIva, connectionString)

            If iva = Nothing Then
                iva = 1
            End If

            Console.WriteLine("Conversione prezzo riuscita")
        Else
            iva = 1
        End If

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "UPDATE Anag_Articoli SET strCodice=@CodArticolo  , strDescrizione = @Descrizione  ,  KeyIdAnag_Um = @UM, dblPrezzo = @Prezzo , KeyIdAnag_Iva  = @IVA   WHERE strCodice=@CodArticolo;"

            mMCm.Parameters.AddWithValue("@CodArticolo", articolo.CodArticolo)
            mMCm.Parameters.AddWithValue("@Descrizione", articolo.Descrizione)
            mMCm.Parameters.AddWithValue("@IVA", iva)
            mMCm.Parameters.AddWithValue("@UM", um)
            mMCm.Parameters.AddWithValue("@Prezzo", prezzo)

            mMyC.Open()

            mMCm.ExecuteNonQuery()
            mMyC.Close()

            Return True

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False
        End Try

    End Function

    Public Function trovaIVA(dblPercentuale As Double, connectionString As String) As Integer

        Dim iva As Integer

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT KeyId FROM Anag_IVA WHERE dblPercentuale = @dblPercentuale"

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@dblPercentuale", dblPercentuale)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read
                iva = If(IsDBNull(mMyR.Item("KeyId").ToString()), Nothing, mMyR.Item("KeyId").ToString())
            Loop

            mMyC.Close()
        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return iva

    End Function

    Public Function trovaUM(codeUM As String, connectionString As String) As Integer   'funzione che trova il keyId tramite il codeUM

        Dim um As Integer

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT KeyId FROM Anag_UM WHERE codeUM = @codeUM"

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@codeUM", codeUM)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read
                um = If(IsDBNull(mMyR.Item("KeyId").ToString()), Nothing, mMyR.Item("KeyId").ToString())
            Loop

            mMyC.Close()
        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return um

    End Function

    '//////////_____________ fine blocco per l'inserimento degli articoli

    '/////////______________ blocco inserimento fornitori / clienti (comprese fatture e destinazioni clienti)


    Function InserisciClientiFornitori(clienteFornitore As ClientiFornitori, connectionString As String) As Boolean

        If clienteFornitore.CarattereRiconoscitore = "C" Then   'se C = cliente altrimenti "F" = fornitore se nessuno di questi restituisce errore , probabilmente per riga vuota

            Try
                mMyC = New SqlConnection(connectionString)

                mMCm = mMyC.CreateCommand
                mMCm.CommandText = "INSERT INTO Anag_Clienti(strCodice, strRagioneSociale, strIndirizzo, strCAP, strCitta, strProvincia, strCodFiscale, strPiva, strTelefono, strCodiceLingua, intLivelloAccesso, bitMagTraceRepo , bitMTAggiornatoRepo) 
                                                    Values (@Codice , @RagioneSociale , @Indirizzo , @Cap, @Citta, @Provincia, @CodFiscale, @PIva, @Telefono, 'IT', 2, 0, 0) "
                'Anag_Articoli , Anag_ArticoliDatiAggiuntivi

                'Assegna i parametri
                mMCm.Parameters.AddWithValue("@Codice", clienteFornitore.Codice)
                mMCm.Parameters.AddWithValue("@RagioneSociale", clienteFornitore.RagioneSociale)
                mMCm.Parameters.AddWithValue("@Indirizzo", clienteFornitore.Indirizzo)
                mMCm.Parameters.AddWithValue("@Cap", clienteFornitore.Cap)
                mMCm.Parameters.AddWithValue("@Citta", clienteFornitore.Citta)
                mMCm.Parameters.AddWithValue("@Provincia", clienteFornitore.Provincia)
                mMCm.Parameters.AddWithValue("@CodFiscale", clienteFornitore.CodFiscale)
                mMCm.Parameters.AddWithValue("@PIva", clienteFornitore.PIva)
                mMCm.Parameters.AddWithValue("@Telefono", clienteFornitore.Telefono)

                Try
                    mMyC.Open()

                    mMCm.ExecuteNonQuery()
                    mMyC.Close()

                Catch ex As Exception
                    Console.Write("Errore Insert : " + ex.Message)
                End Try

                Return True

            Catch Ex As Exception

                Console.Write("non funziona: " + Ex.Message)
                Return False
            End Try

        ElseIf clienteFornitore.CarattereRiconoscitore = "F" Then

            Try
                mMyC = New SqlConnection(connectionString)

                mMCm = mMyC.CreateCommand
                mMCm.CommandText = "INSERT INTO Anag_Fornitori(strCodice, strRagioneSociale, strIndirizzo, strCAP, strCitta, strProvincia, strCodFiscale, strPiva, strTelefono) 
                                                    Values (@Codice , @RagioneSociale , @Indirizzo , @Cap, @Citta, @Provincia, @CodFiscale, @PIva, @Telefono) "


                'Assegna i parametri
                mMCm.Parameters.AddWithValue("@Codice", clienteFornitore.Codice)
                mMCm.Parameters.AddWithValue("@RagioneSociale", clienteFornitore.RagioneSociale)
                mMCm.Parameters.AddWithValue("@Indirizzo", clienteFornitore.Indirizzo)
                mMCm.Parameters.AddWithValue("@Cap", clienteFornitore.Cap)
                mMCm.Parameters.AddWithValue("@Citta", clienteFornitore.Citta)
                mMCm.Parameters.AddWithValue("@Provincia", clienteFornitore.Provincia)
                mMCm.Parameters.AddWithValue("@CodFiscale", clienteFornitore.CodFiscale)
                mMCm.Parameters.AddWithValue("@PIva", clienteFornitore.PIva)
                mMCm.Parameters.AddWithValue("@Telefono", clienteFornitore.Telefono)

                Try
                    mMyC.Open()

                    mMCm.ExecuteNonQuery()
                    mMyC.Close()

                Catch ex As Exception
                    Console.Write("Errore Insert : " + ex.Message)
                End Try

                Return True

            Catch Ex As Exception

                Console.Write("non funziona: " + Ex.Message)
                Return False
            End Try

        End If

    End Function

    Public Function ControlloSpedizioni(connectionString As String) As Integer

        Dim counter As Integer

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT Count(*) as contatore FROM Spedizioni 
                                WHERE bitFileGenerato = 0 and bitRientrato = 0"

            mMyC.Open()

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read
                counter = If(IsDBNull(mMyR.Item("contatore").ToString()), 0, mMyR.Item("contatore").ToString())
            Loop

            mMyC.Close()
        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return counter

    End Function

    Public Function TrovaSpedizioni(ordine As Integer, connectionString As String, numDocumento As Integer) As List(Of Fatture_Gen)

        Dim Fatture As New List(Of Fatture_Gen)

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT 
                                    Spedizioni.keyId as keyIdsped,
                                    Spedizioni.dateData AS data,
                                    COALESCE(Anag_Clienti_Destinazioni.strCap, Anag_Clienti.strCap) AS cap,
                                    COALESCE(Anag_Clienti_Destinazioni.strCitta, Anag_Clienti.strCitta) AS citta,
                                    COALESCE(Anag_Clienti_Destinazioni.strIndirizzo, Anag_Clienti.strIndirizzo) AS indirizzo,
                                    COALESCE(Anag_Clienti_Destinazioni.strProvincia, Anag_Clienti.strProvincia) AS provincia,
                                    Anag_Clienti.strCodice AS codiceCliente,
                                	Anag_Clienti.strRagioneSociale as RagioneSociale,
                                    Spedizioni.dateDataConsegna AS dataConsegna,
                                    Spedizioni.intNumColli AS NumColli
                                FROM Spedizioni
                                JOIN Anag_Clienti ON Spedizioni.KeyIdAnag_Cliente = Anag_Clienti.KeyId
                                left JOIN Anag_Clienti_Destinazioni ON Spedizioni.KeyIdAnag_Clienti_Destinazioni = Anag_Clienti_Destinazioni.KeyId
	                             WHERE Spedizioni.bitFileGenerato = 0 and Spedizioni.bitRientrato = 0"

            mMyC.Open()

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                Dim nuovaFattura As New Fatture_Gen()

                ' Popolamento dei dati
                nuovaFattura.Cap = If(IsDBNull(mMyR.Item("cap")), "", mMyR.Item("cap").ToString())
                nuovaFattura.Citta = If(IsDBNull(mMyR.Item("citta")), "", mMyR.Item("citta").ToString())
                nuovaFattura.codiceCliente = If(IsDBNull(mMyR.Item("codiceCliente")), "", mMyR.Item("codiceCliente").ToString())
                nuovaFattura.DataOra = If(IsDBNull(mMyR.Item("data")), Nothing, Convert.ToDateTime(mMyR.Item("data")))
                nuovaFattura.NumColli = If(IsDBNull(mMyR.Item("NumColli")), "", mMyR.Item("NumColli").ToString())
                nuovaFattura.RagioneSociale = If(IsDBNull(mMyR.Item("RagioneSociale")), "", mMyR.Item("RagioneSociale").ToString())
                nuovaFattura.Indirizzo = If(IsDBNull(mMyR.Item("indirizzo")), "", mMyR.Item("indirizzo").ToString())
                nuovaFattura.Provincia = If(IsDBNull(mMyR.Item("provincia")), "", mMyR.Item("provincia").ToString())
                nuovaFattura.ordine = ordine
                nuovaFattura.keyId = If(IsDBNull(mMyR.Item("keyIdsped")), 0, mMyR.Item("keyIdsped"))
                nuovaFattura.numDoc = numDocumento
                nuovaFattura.sezionale = "M"
                nuovaFattura.tipo = 1
                ' Aggiunta alla lista
                Fatture.Add(nuovaFattura)
                ordine += 1
                numDocumento += 1
            Loop

            mMyC.Close()
        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return Fatture

    End Function

    Public Function TrovaDettagliSpedizione(ordine As Integer, keyId As Integer, connectionString As String) As List(Of Fatture_Righe)

        Dim FattureRighe As New List(Of Fatture_Righe)
        Dim Nriga As Integer = 1
        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT 
                                Spedizioni.intAnno as anno,
                                Spedizioni_Dettagli.strCodiceArticolo as CodiceArticolo,
                                Spedizioni_Dettagli.strDescrizioneArticolo as DescrizioneArticolo,
                                COALESCE(
                                    TracciabilitaAddizionale.strLotto, 
                                    Tracc2.strLotto, 
                                    Tracc3.strLotto, 
                                    Tracc4.strLotto, 
                                    Tracc5.strLotto, 
                                    Tracc6.strLotto, 
                                    Tracc7.strLotto, 
                                    Tracc8.strLotto, 
                                    Tracc9.strLotto
                                ) as Lotto,
                                Spedizioni_Dettagli.dblQuantita as qta,
                                Spedizioni_Dettagli.dblPrezzoUnit as prezzoUnit,
                                Spedizioni_Dettagli.dblPercentualeIva as percIva,
                                Anag_Um.codeUm as um,
                                Spedizioni_Dettagli.percSco1 as sconto,
                                Spedizioni_Dettagli.percSco2 as sconto2,
                                Spedizioni_Dettagli.percSco3 as sconto3
                            FROM Spedizioni
                            JOIN Spedizioni_Dettagli ON Spedizioni.KeyId = Spedizioni_Dettagli.KeyIdSpedizioni
                            JOIN Anag_Um ON Spedizioni_Dettagli.KeyIdAnag_Um = Anag_Um.keyId

                            --  Conferimenti
                            LEFT JOIN Conferimenti ON Spedizioni_Dettagli.KeyIdConferimenti = Conferimenti.keyId
                            LEFT JOIN TracciabilitaAddizionale ON Conferimenti.KeyIdTracciabilitaAddizionale = TracciabilitaAddizionale.keyId

                            --  Sezionamenti
                            LEFT JOIN Sezionamenti ON Spedizioni_Dettagli.KeyIdSezionamenti = Sezionamenti.keyId
                            LEFT JOIN LavorazioniFasi_IngressoLotti ON Sezionamenti.KeyId = LavorazioniFasi_IngressoLotti.KeyIdSezionamenti
                            LEFT JOIN TracciabilitaAddizionale Tracc2 ON LavorazioniFasi_IngressoLotti.KeyIdTracciabilitaAddizionale = Tracc2.keyId

                            -- Parzializzazione
                            LEFT JOIN InParzializzazione_Esecuzione ON Spedizioni_Dettagli.KeyIdInParzializzazione_Esecuzione = InParzializzazione_Esecuzione.KeyId
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso2 ON InParzializzazione_Esecuzione.KeyId = Ingresso2.KeyIdConferimenti
                            LEFT JOIN Conferimenti Conferimenti2 ON Ingresso2.KeyIdConferimenti = Conferimenti2.keyId
                            LEFT JOIN TracciabilitaAddizionale Tracc3 ON Conferimenti2.KeyIdTracciabilitaAddizionale = Tracc3.keyId

                            -- Cartoni/Pallets
                            LEFT JOIN CartonePallets ON Spedizioni_Dettagli.KeyIdCartonePallets = CartonePallets.KeyId
                            LEFT JOIN TracciabilitaAddizionale Tracc4 ON CartonePallets.KeyIdTraccAddizionale = Tracc4.keyId

                            -- Destinazioni
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso3 ON Ingresso3.KeyId = Spedizioni_Dettagli.KeyIdLavorazioniFasi_Destinazione
                            LEFT JOIN TracciabilitaAddizionale Tracc5 ON Ingresso3.KeyIdTracciabilitaAddizionale = Tracc5.keyId

                            -- Insacco
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso4 ON Ingresso4.KeyId = Spedizioni_Dettagli.KeyIdLavorazioniFasi_Insacco
                            LEFT JOIN TracciabilitaAddizionale Tracc6 ON Ingresso4.KeyIdTracciabilitaAddizionale = Tracc6.keyId

                            -- Confezionamento
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso5 ON Ingresso5.KeyId = Spedizioni_Dettagli.KeyIdLavorazioniFasi_ConfezionamentoDArtOut
                            LEFT JOIN TracciabilitaAddizionale Tracc7 ON Ingresso5.KeyIdTracciabilitaAddizionale = Tracc7.keyId

                            -- Impasti
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso6 ON Ingresso6.KeyId = Spedizioni_Dettagli.KeyIdLavorazioniFasi_Impasti
                            LEFT JOIN TracciabilitaAddizionale Tracc8 ON Ingresso6.KeyIdTracciabilitaAddizionale = Tracc8.keyId

                            -- Ricette
                            LEFT JOIN LavorazioniFasi_IngressoLotti Ingresso7 ON Ingresso7.KeyId = Spedizioni_Dettagli.KeyIdLavorazioniFasi_Ricette
                            LEFT JOIN TracciabilitaAddizionale Tracc9 ON Ingresso7.KeyIdTracciabilitaAddizionale = Tracc9.keyId
                            
                            WHERE Spedizioni.KeyId = @keyId"

            mMCm.Parameters.AddWithValue("@keyId", keyId)
            mMyC.Open()

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                Dim riga As New Fatture_Righe With {
                 .Anno = If(IsDBNull(mMyR.Item("anno")), 0, mMyR.Item("anno")),
                 .CodiceArticolo = If(IsDBNull(mMyR.Item("CodiceArticolo").ToString()), "", mMyR.Item("CodiceArticolo").ToString()),
                 .DescrizioneArticolo = If(IsDBNull(mMyR.Item("DescrizioneArticolo").ToString()), "", mMyR.Item("DescrizioneArticolo").ToString()),
                 .Lotto = If(IsDBNull(mMyR.Item("Lotto").ToString()), "", mMyR.Item("Lotto").ToString()),
                 .NumRiga = Nriga,
                 .Ordine = ordine,
                 .PrezzoUnit = If(IsDBNull(mMyR.Item("prezzoUnit")), 0, mMyR.Item("prezzoUnit")),
                 .Qta = If(IsDBNull(mMyR.Item("qta")), 0, mMyR.Item("qta")),
                 .Um = If(IsDBNull(mMyR.Item("Um")), "", mMyR.Item("Um")),
                 .Sconto = If(IsDBNull(mMyR.Item("sconto")), 0, mMyR.Item("sconto")),
                 .Sconto2 = If(IsDBNull(mMyR.Item("sconto2")), 0, mMyR.Item("sconto2")),
                 .Sconto3 = If(IsDBNull(mMyR.Item("sconto3")), 0, mMyR.Item("sconto3")).ToString,
                 .Iva = If(IsDBNull(mMyR.Item("percIva")), 0, mMyR.Item("percIva"))
                }
                FattureRighe.Add(riga)
                Nriga += 1

            Loop

            mMyC.Close()
        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return FattureRighe

    End Function

    Public Function TrovaTotaleSpedizioneDaKeyID(keyId As Integer, connectionString As String) As Double()

        Dim totale(1) As Double

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "SELECT SUM(Spedizioni_Dettagli.dblPrezzoUnit * dblQuantita) as TotaleSenzaIva,
	                            SUM(Spedizioni_Dettagli.dblPrezzoUnitIva * dblQuantita) as TotaleConIva
                                 FROM Spedizioni
	                             JOIN Spedizioni_Dettagli ON Spedizioni.KeyId = Spedizioni_Dettagli.KeyIdSpedizioni
	                             WHERE Spedizioni.KeyId = @keyId"

            mMCm.Parameters.AddWithValue("@keyId", keyId)
            mMyC.Open()

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                totale(0) = If(IsDBNull(mMyR.Item("TotaleSenzaIva").ToString()), Nothing, mMyR.Item("TotaleSenzaIva").ToString())
                totale(1) = If(IsDBNull(mMyR.Item("TotaleConIva").ToString()), Nothing, mMyR.Item("TotaleConIva").ToString())
            Loop

            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

        End Try

        Return totale

    End Function

    Public Sub FileGeneratoTrue(keyId As Integer, connectionString As String)

        Try

            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "UPDATE spedizioni SET bitFileGenerato = 1 WHERE keyId = @keyId"

            mMCm.Parameters.AddWithValue("@keyId", keyId)

            mMyC.Open()

            mMCm.ExecuteNonQuery()
            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Dim fileLog As System.IO.StreamWriter
            fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLog.WriteLine(Ex.Message.ToString)   'Log
            fileLog.Close()
        End Try

    End Sub

    Public Function ClienteFornitoreGiaPresente(codice As String, CarattereRiconoscitore As String, connectionString As String) As Boolean

        Dim esistente As Boolean = False

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand

            If CarattereRiconoscitore = "C" Then

                mMCm.CommandText = "SELECT strCodice FROM Anag_Clienti WHERE strCodice = @Codice"

            ElseIf CarattereRiconoscitore = "F" Then

                mMCm.CommandText = "SELECT strCodice FROM Anag_Fornitori WHERE strCodice = @Codice"

            End If

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@Codice", codice)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                If If(IsDBNull(mMyR.Item("strCodice").ToString()), Nothing, mMyR.Item("strCodice").ToString()) <> Nothing Then
                    esistente = True
                Else
                    esistente = False
                End If

            Loop

            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False

        End Try

        Return esistente

    End Function

    Public Function UpdateClienteFornitore(clienteFornitore As ClientiFornitori, connectionString As String)

        Try


            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand
            If clienteFornitore.CarattereRiconoscitore = "C" Then
                mMCm.CommandText = "UPDATE Anag_Clienti SET strCodice = @Codice, strRagioneSociale =  @RagioneSociale, strIndirizzo = @Indirizzo, strCAP = @Cap, strCitta = @Citta, strProvincia = @Provincia, strCodFiscale =  @CodFiscale, strPiva = @PIva, strTelefono =@Telefono WHERE strCodice = @Codice"
            ElseIf clienteFornitore.CarattereRiconoscitore = "F" Then
                mMCm.CommandText = "UPDATE Anag_Fornitori SET strCodice = @Codice, strRagioneSociale =  @RagioneSociale, strIndirizzo = @Indirizzo, strCAP = @Cap, strCitta = @Citta, strProvincia = @Provincia, strCodFiscale =  @CodFiscale, strPiva = @PIva, strTelefono =@Telefono WHERE strCodice = @Codice"
            End If


            mMCm.Parameters.AddWithValue("@Codice", clienteFornitore.Codice)
            mMCm.Parameters.AddWithValue("@RagioneSociale", clienteFornitore.RagioneSociale)
            mMCm.Parameters.AddWithValue("@Indirizzo", clienteFornitore.Indirizzo)
            mMCm.Parameters.AddWithValue("@Cap", clienteFornitore.Cap)
            mMCm.Parameters.AddWithValue("@Citta", clienteFornitore.Citta)
            mMCm.Parameters.AddWithValue("@Provincia", clienteFornitore.Provincia)
            mMCm.Parameters.AddWithValue("@CodFiscale", clienteFornitore.CodFiscale)
            mMCm.Parameters.AddWithValue("@PIva", clienteFornitore.PIva)
            mMCm.Parameters.AddWithValue("@Telefono", clienteFornitore.Telefono)

            mMyC.Open()

            mMCm.ExecuteNonQuery()
            mMyC.Close()

            Return True

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)

            Return False
        End Try

    End Function

    Public Function TrovaKeyIdClienteDaCodice(codice As String, connectionString As String) As Integer

        Dim keyId As Integer

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand


            mMCm.CommandText = "SELECT distinct keyId FROM Anag_Clienti WHERE strcodice = @codice"

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@codice", codice)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                keyId = If(IsDBNull(mMyR.Item("keyId").ToString()), Nothing, mMyR.Item("keyId").ToString())

            Loop

            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False

        End Try

        Return keyId

    End Function

    Public Function DestinazioneGiaPresente(keyIdCliente As Integer, codiceDestinazione As Integer, connectionString As String) As Boolean

        Dim esistente As Boolean = False

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand


            mMCm.CommandText = "SELECT distinct strCodice FROM Anag_Clienti_Destinazioni WHERE KeyIdAnag_Clienti = @keyId and strcodice = @codice"

            mMyC.Open()
            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@keyId", keyIdCliente)
            mMCm.Parameters.AddWithValue("@codice", codiceDestinazione)

            mMyR = mMCm.ExecuteReader()


            Do While mMyR.Read

                If If(IsDBNull(mMyR.Item("strCodice").ToString()), Nothing, mMyR.Item("strCodice").ToString()) <> Nothing Then
                    esistente = True
                Else
                    esistente = False
                End If

            Loop

            mMyC.Close()

        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False

        End Try

        Return esistente
    End Function

    Public Function InserisciDestinazioneClienti(keyIdAnagClienti As Integer, destinazione As DestinazioniClienti, connectionString As String) As Boolean

        Try
            mMyC = New SqlConnection(connectionString)

            mMCm = mMyC.CreateCommand
            mMCm.CommandText = "INSERT INTO Anag_Clienti_Destinazioni(keyIdAnag_Clienti, strCodice, strIndirizzo, strCAP, strCitta, strProvincia, bitMagTraceRepo) 
                                Values (@keyIdAnagClienti , @codice , @Indirizzo , @Cap, @Citta, @Provincia, 0) "


            'Assegna i parametri
            mMCm.Parameters.AddWithValue("@keyIdAnagClienti", keyIdAnagClienti)
            mMCm.Parameters.AddWithValue("@Codice", destinazione.CodiceDestinazione)
            mMCm.Parameters.AddWithValue("@Indirizzo", destinazione.Indirizzo)
            mMCm.Parameters.AddWithValue("@Citta", destinazione.Citta)
            mMCm.Parameters.AddWithValue("@Cap", destinazione.cap)
            mMCm.Parameters.AddWithValue("@Provincia", destinazione.provincia)

            Try
                mMyC.Open()

                mMCm.ExecuteNonQuery()
                mMyC.Close()
                Return True
            Catch ex As Exception
                Console.Write("Errore Insert : " + ex.Message)
            End Try



        Catch Ex As Exception

            Console.Write("non funziona: " + Ex.Message)
            Return False
        End Try

    End Function

    Public Function UpdateDestinazioneClienti(keyIdAnagClienti As Integer, codiceDestinazione As Integer, destinazione As DestinazioniClienti, connectionString As String) As Boolean
        Try
            mMyC = New SqlConnection(connectionString)
            mMCm = mMyC.CreateCommand

            mMCm.CommandText = "UPDATE Anag_Clienti_Destinazioni 
                            SET strIndirizzo = @Indirizzo, 
                                strCAP = @Cap, 
                                strCitta = @Citta, 
                                strProvincia = @Provincia 
                            WHERE keyIdAnag_Clienti = @keyIdAnagClienti AND strCodice = @Codice"

            ' Assegna i parametri
            mMCm.Parameters.AddWithValue("@keyIdAnagClienti", keyIdAnagClienti)
            mMCm.Parameters.AddWithValue("@Codice", codiceDestinazione)
            mMCm.Parameters.AddWithValue("@Indirizzo", destinazione.Indirizzo)
            mMCm.Parameters.AddWithValue("@Cap", destinazione.cap)
            mMCm.Parameters.AddWithValue("@Citta", destinazione.Citta)
            mMCm.Parameters.AddWithValue("@Provincia", destinazione.provincia)

            Try
                mMyC.Open()
                mMCm.ExecuteNonQuery()
                mMyC.Close()
            Catch ex As Exception
                Console.Write("Errore Update: " + ex.Message)
                Return False
            End Try

            Return True

        Catch ex As Exception
            Console.Write("Errore generale Update: " + ex.Message)
            Return False
        End Try
    End Function




    Private Function DatabaseModule_InserisciClientiFornitori1(clienteFornitore As ClientiFornitori, connectionString As String) As Boolean Implements DatabaseModule.InserisciClientiFornitori
        Throw New NotImplementedException()
    End Function

    Private Function DatabaseModule_ArticoloGiaPresente(CodArticolo As String, connectionString As String) As Boolean Implements DatabaseModule.ArticoloGiaPresente
        Throw New NotImplementedException()
    End Function

    Private Function DatabaseModule_UpdateArticolo(articolo As Articoli, connectionString As String) As Object Implements DatabaseModule.UpdateArticolo
        Throw New NotImplementedException()
    End Function

    Private Function DatabaseModule_ClienteFornitoreGiaPresente(codice As String, CarattereRiconoscitore As String, connectionString As String) As Boolean Implements DatabaseModule.ClienteFornitoreGiaPresente
        Throw New NotImplementedException()
    End Function

    Private Function DatabaseModule_UpdateClienteFornitore(clienteFornitore As ClientiFornitori, connectionString As String) As Object Implements DatabaseModule.UpdateClienteFornitore
        Throw New NotImplementedException()
    End Function


    '/////////______________ fine blocco inserimento fornitori / clienti 

End Class
