Imports System.IO
Imports System.Security
Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports DbfDataReader
Imports DotNetDBF
Imports SocialExplorer.IO
Imports SocialExplorer.IO.FastDBF
Imports DevComponents.AdvTree
Imports System.Xml
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window
Imports MS.Internal.Xml
Imports System.Reflection
Imports System.Text
Imports System.Runtime.InteropServices.ComTypes
Imports DevComponents.DotNetBar
Imports System.Runtime.InteropServices

Public Class FrmMain
    'Dim mUltimaSpedizione As Integer
    'Dim mNewUltimaSpedizione As Integer

    Dim giorniEliminazioneFile As Integer
    Dim mModel As New ModelFunction()
    Dim config As New Conf()
    Dim mConnectionString As String
    Dim mPathArticoli As String
    Dim mPathClientiFornitori As String
    Dim mDataDestinazioniClienti As DateTime
    Dim mPathFattGen As String
    Dim mPathFattRIg As String
    Dim mPathDestinazioni As String

    Dim mArticoliList As New List(Of Articoli)
    Dim mClientiFornitoriList As New List(Of ClientiFornitori)
    Dim mDestinazioniList As New List(Of DestinazioniClienti)

    Dim mTempoFatt As Integer
    Dim mTempo As Integer
    Dim mDataArticoli As DateTime
    Dim mDataClientiFornitori As DateTime
    Dim mFlagModificatoArticoli As Boolean
    Dim mFlagModificatoClientiFornitori As Boolean
    Dim mFlagModificatoDestinazioniClienti As Boolean


    Dim mPathAttualeLog As String = Path.Combine(Application.StartupPath + "/Log", $"log_{DateTime.Now:yyyy-MM-dd}.txt")
    Dim mPathCartellaLog As String = Path.Combine(Application.StartupPath, "Log")
    Private dataUltimoLog As Date = Date.Today
    Dim mEsporta As New EsportaFileDBF
    Dim mImporta As New ImportaFileDBF
    Dim mOrdine As Integer
    Dim cdxcre As New CDXCreator

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim exePath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim configPath As String = Path.Combine(exePath, "ImportaDBF.exe.config")
        Dim doc As New XmlDocument()
        doc.Load(configPath)

        Dim nodes As XmlNodeList = doc.SelectNodes("/configuration/appSettings/add")

        For Each node As XmlNode In nodes
            Dim key As String = node.Attributes("key")?.Value
            Dim value As String = node.Attributes("value")?.Value

            If key = "PathArticoli" Then
                mPathArticoli = value
            ElseIf key = "PathClientiFornitori" Then
                mPathClientiFornitori = value
            ElseIf key = "connectionString" Then
                mConnectionString = value
            ElseIf key = "Time" Then
                mTempo = value
            ElseIf key = "TimeFatt" Then
                mTempoFatt = value
            ElseIf key = "DataArticoli" Then
                mDataArticoli = value
            ElseIf key = "DataCliForn" Then
                mDataClientiFornitori = value
            ElseIf key = "DataDestinazioni" Then
                mDataDestinazioniClienti = value
            ElseIf key = "PathDestinazioni" Then
                mPathDestinazioni = value
            ElseIf key = "PathFattGen" Then
                mPathFattGen = value
            ElseIf key = "PathFattRig" Then
                mPathFattRIg = value
            ElseIf key = "GiorniPerEliminazioneFile" Then
                giorniEliminazioneFile = CInt(value)
            End If
        Next

        EliminaVecchiFile()

        txtTempoEliminazioneFile.Text = giorniEliminazioneFile
        txtArticoli.Text = mPathArticoli
        txtClientiFornitori.Text = mPathClientiFornitori
        txtDestinazioniClienti.Text = mPathDestinazioni
        txtFattureGen.Text = mPathFattGen
        txtFattureRig.Text = mPathFattRIg
        TimerRefreshImportazione.Interval = mTempo * 1000

        txtTempoAggiornamentoFatt.Text = mTempoFatt
        TimerRefreshFatture.Interval = mTempoFatt * 1000

        ' TimerRefreshImportazione.Enabled = True

        TimerControllerLog.Interval = 5 * 60 * 1000 ' ogni 5 minuti
        TimerControllerLog.Start()
        ' TimerControllerLog.Enabled = True

        ' Calcola i minuti e i secondi rimanenti
        Dim minuti As Integer = mTempo \ 60
        Dim secondi As Integer = mTempo Mod 60

        Dim minutifatt As Integer = mTempoFatt \ 60
        Dim secondifatt As Integer = mTempoFatt Mod 60

        ' Mostra il risultato nella textBox (minuti e secondi)
        txtTempo.Text = String.Format("{0}.{1}", minuti, secondi)
        txtTempoAggiornamentoFatt.Text = String.Format("{0}.{1}", minutifatt, secondifatt)


        'Dim percorsoEseguibile As String = Application.StartupPath
        'Dim percorsoScript As String = Path.Combine(percorsoEseguibile, "scriptFattgen.prg")
        'cdxcre.EseguiFoxPro(percorsoScript, "C:\Program Files (x86)\Microsoft Visual FoxPro 9\vfp9.exe")

    End Sub


    Private Sub BtnSfogliaX_Click(sender As Object, e As EventArgs) Handles btnRicaricaClientiFornitori.Click

        ClientiFornitori(True)
        DestinazioniClienti(True)

    End Sub


    Private Sub btnRicaricaArticoli_Click(sender As Object, e As EventArgs) Handles btnRicaricaArticoli.Click
        articoli(True)
    End Sub


    Public Sub articoli(byPass As Boolean)    '///------------------Articoli 

        Dim CSVSuccess As Boolean
        Dim counterArticoliModificati As Integer = 0
        Dim counterArticoliInseriti As Integer = 0
        Dim counterArticoliInErrore As Integer = 0

        If byPass = False Then

            mFlagModificatoArticoli = ControllaDataFile(mPathArticoli, mDataArticoli, "DataArticoli")  'l'ultimo campo è il nome della key nel file xml 
        Else
            Dim fileDate As DateTime = System.IO.File.GetLastWriteTime(mPathArticoli)
            printDataOnXML("DataArticoli", fileDate)

            mFlagModificatoArticoli = True
        End If


        If mFlagModificatoArticoli = True Then

            Try
                mArticoliList.Clear()

                'Dim sr As New StreamReader(Dialog.FileName)
                Dim successInsert As Boolean
                Dim successUpdate As Boolean = False


                '   articoliList = exp.EstraiDatiArticoliDBF(Dialog.FileName)

                CSVSuccess = mImporta.ConvertDBFtoCSV(txtArticoli.Text)  ' conversione da DBF a CSV

                If CSVSuccess <> True Then

                    Dim fileLog As System.IO.StreamWriter
                    fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                    fileLog.WriteLine("Errore durante l' importazione articoli il " + Now.ToString)
                    fileLog.Close()
                    Exit Sub
                End If

                mArticoliList = mImporta.LeggiArticoliDaCSV(txtArticoli.Text + ".csv")   'il path è quello del file dbf ma con formato csv


                Dim i As Integer = 0

                If mArticoliList Is Nothing Then
                    MessageBox.Show("Il files non contiene dati corretti, controllare il tipo di files selezionato")
                    Exit Sub
                End If

                For Each articolo In mArticoliList  'scorre la lista , visualizza i dati e inserisce nel db



                    If i = 0 Then

                        i += 1 'salta la prima riga che contiene la sola scritta "Azienda.db"

                    Else

                        If articolo.CodArticolo <> "" Then


                            successUpdate = mModel.articoloGiaPresente(articolo.CodArticolo, mConnectionString)

                            If successUpdate = False Then
                                successInsert = mModel.InserisciArticolo(articolo, mConnectionString) 'funzione per l'inserimento nel DB

                                If successInsert Then
                                    Console.WriteLine("Articolo aggiunto.")
                                    counterArticoliInseriti += 1
                                Else
                                    Console.WriteLine("Errore.")
                                    counterArticoliInErrore += 1
                                End If

                            Else

                                successInsert = mModel.UpdateArticolo(articolo, mConnectionString)

                                If successInsert Then
                                    Console.WriteLine("Articolo modificato.")
                                    counterArticoliModificati += 1
                                Else
                                    counterArticoliInErrore += 1
                                End If

                            End If

                        End If

                    End If
                Next

            Catch ex As Exception

                Dim itemNonRiuscito As New ListViewItem

                itemNonRiuscito.Text = "Importazione non riuscita il " + Now.ToString + ": " + ex.Message

                lvwTrasmettitore.Items.Add(itemNonRiuscito)

                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine(itemNonRiuscito.Text)
                fileLog.Close()

                Exit Sub

            End Try

            Dim itemTrasmettitore As New ListViewItem

            '-- Facciamo mArticoliList.Count - 2 perchè dobbiamo contare la prima riga in alto che è Azienda.Db e l'ultima in basso che è sempre vuota

            itemTrasmettitore.Text = "Importazione articoli effettuata con successo il " + Now.ToString + "(" + (mArticoliList.Count - 2).ToString + " trovati - " + counterArticoliInseriti.ToString + " inseriti - " + counterArticoliModificati.ToString + " modificati - " + counterArticoliInErrore.ToString + " in errore)"

            lvwTrasmettitore.Items.Add(itemTrasmettitore)

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(itemTrasmettitore.Text)
            fileLo.Close()

        End If


    End Sub

    Public Sub ClientiFornitori(byPass As Boolean) '///------------------Clienti - Fornitori 
        Dim CSVSuccess As Boolean
        Dim counterClientiFornitoriModificati As Integer = 0
        Dim counterClientiFornitoriInseriti As Integer = 0
        Dim counterClientiFornitoriInErrore As Integer = 0

        If byPass = False Then
            mFlagModificatoClientiFornitori = ControllaDataFile(mPathClientiFornitori, mDataClientiFornitori, "DataCliForn")   'l'ultimo campo è il nome della key nel file xml 
        Else

            Dim fileDate As DateTime = System.IO.File.GetLastWriteTime(mPathClientiFornitori)
            printDataOnXML("DataCliForn", fileDate)

            mFlagModificatoClientiFornitori = True

        End If


        If mFlagModificatoClientiFornitori = True Then

            Try

                mClientiFornitoriList.Clear()


                'exp.EstraiNomiTabelle(Dialog.FileName)
                '  ClientiFornitoriList = exp.EstraiDatiClientiFornitoriDBF(Dialog.FileName)

                Dim successInsert As Boolean
                Dim successUpdate As Boolean = False

                CSVSuccess = mImporta.ConvertDBFtoCSV(txtClientiFornitori.Text)  ' conversione da DBF a CSV

                If CSVSuccess <> True Then

                    Dim fileLog As System.IO.StreamWriter
                    fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                    fileLog.WriteLine("Errore durante l' importazione clienti/fornitori il " + Now.ToString)
                    fileLog.Close()
                    Exit Sub
                End If

                mClientiFornitoriList = mImporta.LeggiClientiFornitoriDaCSV(txtClientiFornitori.Text + ".csv")  'il path è quello del file dbf ma con formato csv

                Dim i = 0

                If mClientiFornitoriList Is Nothing Then
                    Dim fileLog As System.IO.StreamWriter
                    fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                    fileLog.WriteLine("Apertura del file Clifor.DBF non riuscita.")
                    fileLog.Close()
                    Exit Sub
                End If

                For Each ClienteFornitore In mClientiFornitoriList  'scorre la lista , visualizza i dati e inserisce nel db

                    If i = 0 Then

                        i += 1   'salta la prima riga che contiene la sola scritta "Azienda.db"

                    Else

                        If ClienteFornitore.Codice <> "" Then

                            successUpdate = mModel.ClienteFornitoreGiaPresente(ClienteFornitore.Codice, ClienteFornitore.CarattereRiconoscitore, mConnectionString)

                            If successUpdate = False Then

                                successInsert = mModel.InserisciClientiFornitori(ClienteFornitore, mConnectionString) 'funzione per l'inserimento nel DB

                                If successInsert Then
                                    Console.WriteLine("Cliente/fornitore aggiunto.")
                                    counterClientiFornitoriInseriti += 1
                                Else
                                    Console.WriteLine("Errore.")
                                    counterClientiFornitoriInErrore += 1
                                End If

                            Else

                                successInsert = mModel.UpdateClienteFornitore(ClienteFornitore, mConnectionString)

                                If successInsert Then
                                    Console.WriteLine("Cliente/fornitore modificato.")
                                    counterClientiFornitoriModificati += 1
                                Else
                                    counterClientiFornitoriInErrore += 1
                                End If

                            End If

                        End If

                    End If

                Next

            Catch ex As Exception

                Dim itemNonRiuscito As New ListViewItem

                itemNonRiuscito.Text = "Importazione clienti / fornitori non riuscita il " + Now.ToString + ": " + ex.Message

                lvwTrasmettitore.Items.Add(itemNonRiuscito)

                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine(itemNonRiuscito.Text)
                fileLog.Close()

                Exit Sub

            End Try

            If counterClientiFornitoriInseriti > 0 Then
                counterClientiFornitoriInseriti -= 1
            End If

            If counterClientiFornitoriModificati > 0 Then
                counterClientiFornitoriModificati -= 1
            End If

            '-- Facacimo i contatori -1 per la riga vuota in basso
            '-- Facciamo mClientiFornitoriList.Count - 2 perchè dobbiamo contare la prima riga in alto che è Azienda.Db e l'ultima in basso che è sempre vuota

            Dim itemTrasmettitore As New ListViewItem

            itemTrasmettitore.Text = "Importazione Clienti/fornitori effettuata con successo il " + Now.ToString + "(" + (mClientiFornitoriList.Count - 2).ToString + " trovati - " + counterClientiFornitoriInseriti.ToString + " inseriti - " + counterClientiFornitoriModificati.ToString + " modificati - " + counterClientiFornitoriInErrore.ToString + " in errore)"

            lvwTrasmettitore.Items.Add(itemTrasmettitore)

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(itemTrasmettitore.Text)
            fileLo.Close()

        End If

    End Sub

    Public Sub DestinazioniClienti(byPass As Boolean)
        Dim CSVSuccess As Boolean
        Dim keyIdCliente As Integer
        Dim FlagCliente As Boolean
        Dim counterDestinazioniModificate As Integer = 0
        Dim counterDestinazioniInseriti As Integer = 0
        Dim counterDestinazioniInErrore As Integer = 0
        mDestinazioniList.Clear()


        If byPass = False Then

            mFlagModificatoClientiFornitori = ControllaDataFile(mPathDestinazioni, mDataDestinazioniClienti, "DataDestinazioni")   'l'ultimo campo è il nome della key nel file xml 

        Else

            Dim fileDate As DateTime = System.IO.File.GetLastWriteTime(mPathDestinazioni)
            printDataOnXML("DataDestinazioni", fileDate)

            mFlagModificatoClientiFornitori = True

        End If

        If mFlagModificatoClientiFornitori = True Then

            Dim i = 1
            Dim effettiveDestinazioniClienti As Integer = 0
            Dim successInsert As Boolean
            Dim successUpdate As Boolean = False

            CSVSuccess = mImporta.ConvertDBFtoCSV(txtDestinazioniClienti.Text)  ' conversione da DBF a CSV

            If CSVSuccess <> True Then
                Dim itemTrasmettitor As New ListViewItem
                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                itemTrasmettitor.Text = "Errore durante l' importazione articoli , file csv non creato il " + Now.ToString
                fileLog.WriteLine(itemTrasmettitor.Text)
                fileLog.Close()
                Exit Sub
            End If

            mDestinazioniList = mImporta.LeggiDestinazioniClientiDaCSV(txtDestinazioniClienti.Text + ".csv")

            If mDestinazioniList.Count = 0 Then
                Dim itemTrasmettitor As New ListViewItem
                itemTrasmettitor.Text = "Il file CSV destinazioni non ha prodotto risultati il " + Now.ToString
                lvwTrasmettitore.Items.Add(itemTrasmettitor)
                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine(itemTrasmettitor)
                fileLog.Close()
            End If

            Try


                For Each destinazione In mDestinazioniList

                    If i = 1 Then

                        i += 1

                    Else

                        FlagCliente = mModel.ClienteFornitoreGiaPresente(destinazione.CodiceCliente, "C", mConnectionString)

                        If destinazione.CodiceDestinazione <> Nothing And FlagCliente = True Then
                            effettiveDestinazioniClienti += 1
                            keyIdCliente = mModel.TrovaKeyIdClienteDaCodice(destinazione.CodiceCliente, mConnectionString)

                            successUpdate = mModel.DestinazioneGiaPresente(keyIdCliente, destinazione.CodiceDestinazione, mConnectionString)

                            If successUpdate = False Then

                                successInsert = mModel.InserisciDestinazioneClienti(keyIdCliente, destinazione, mConnectionString) 'insert 'funzione per l'inserimento nel DB

                                If successInsert Then
                                    Console.WriteLine("Destinazione aggiunto.")
                                    counterDestinazioniInseriti += 1
                                Else
                                    Console.WriteLine("Errore Destinazione.")
                                    counterDestinazioniInErrore += 1
                                End If

                            Else

                                successInsert = mModel.UpdateDestinazioneClienti(keyIdCliente, destinazione.CodiceDestinazione, destinazione, mConnectionString) 'update nel db

                                If successInsert Then
                                    Console.WriteLine("Destinazione modificato.")
                                    counterDestinazioniModificate += 1
                                Else
                                    counterDestinazioniInseriti += 1
                                End If

                            End If

                        End If
                    End If
                Next

            Catch ex As Exception

                Dim itemNonRiuscito As New ListViewItem

                itemNonRiuscito.Text = "Importazione destinazioni non riuscita il " + Now.ToString + ": " + ex.Message

                Dim fileLog As System.IO.StreamWriter
                fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                fileLog.WriteLine(itemNonRiuscito.Text)
                fileLog.Close()
                lvwTrasmettitore.Items.Add(itemNonRiuscito)

                Exit Sub

            End Try

            Dim itemTrasmettitore As New ListViewItem

            itemTrasmettitore.Text = "Importazione Destinazioni effettuata con successo il " + Now.ToString + "(" + effettiveDestinazioniClienti.ToString + " trovati - " + counterDestinazioniInseriti.ToString + " inseriti - " + counterDestinazioniModificate.ToString + " modificati - " + counterDestinazioniInErrore.ToString + " in errore)"

            lvwTrasmettitore.Items.Add(itemTrasmettitore)

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(itemTrasmettitore.Text)
            fileLo.Close()


        End If

    End Sub

    Public Sub Fatture()
        Dim copiato As Boolean
        Dim cont = mModel.ControlloSpedizioni(mConnectionString) 'controllo preventivo sulle spedizioni

        If cont = 0 Then 'esce se non trova alcuna spedizione 

            Dim itemTrasmettitore As New ListViewItem
            itemTrasmettitore.Text = "Nessuna nuova spedizione trovata il " + Now.ToString
            lvwTrasmettitore.Items.Add(itemTrasmettitore)

            Exit Sub

        End If

        Dim numDocumento As Integer
        Dim Fatture As New List(Of Fatture_Gen)
        Dim Fatture_righe As New List(Of Fatture_Righe)
        Dim rigaCompletaGen As Boolean = False
        Dim righeCompletaRig As Boolean = False

        copiato = mImporta.CopiaFile(txtFattureRig.Text) 'il file fat_rig viene copiato , il file fat_gen viene copiato invece quando viene preso l'ordine

        If copiato <> True Then
            Dim fileLog As System.IO.StreamWriter
            fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLog.WriteLine("L'esportazione delle fatture non è avvenuta a causa di un problema durante la copia del file Fatt_Rig il " + Now.ToString)
            fileLog.Close()
        End If

        mOrdine = mEsporta.trovaUltimoOrdine(txtFattureGen.Text)
        numDocumento = mEsporta.trovaUltimoNumeroDocumento(txtFattureGen.Text)
        'Dim fileLo As System.IO.StreamWriter  'apro il file che verrà utilizzato per loggare
        'fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)

        Try

            Fatture = mModel.TrovaSpedizioni(mOrdine, mConnectionString, numDocumento)

            Dim itemTrasmettitorerig As New ListViewItem
            Dim itemTrasmettitoreGen As New ListViewItem

            Dim i As Integer = 0

            '//--per le colonne:
            Dim listSuccessGem As New List(Of Integer)
            '//--per le righe:
            Dim listSuccessRig As New List(Of Integer)

            Dim Totale(1) As Double   'array per i totali con iva e senza iva

            Dim counterRig As Integer = 0


            For Each Fattura In Fatture                                                                                               'esporta le fatture e tiene il counter delle righe riuscite

                Totale = mModel.TrovaTotaleSpedizioneDaKeyID(Fattura.keyId, mConnectionString)            'trova i totali della fattura che stiamo per importare con indice 0 sarà senza iva , 1 con iva
                rigaCompletaGen = mEsporta.EsportaFatturaGen(Fattura.ordine, txtFattureGen.Text, Fattura, Totale(0), Totale(1))

                If rigaCompletaGen = True Then

                    listSuccessGem.Add(Fattura.keyId) 'successo

                    Fatture_righe = mModel.TrovaDettagliSpedizione(Fattura.ordine, Fattura.keyId, mConnectionString)              'trova i dettagli delle spedizioni 

                    For Each riga In Fatture_righe                                                                          'scorre i dettagli e aggiunge in fatt_rig 

                        righeCompletaRig = mEsporta.EsportaFatturaRig(riga.Ordine, txtFattureRig.Text, riga)

                        If righeCompletaRig = True Then

                            listSuccessRig.Add(Fattura.keyId)

                        Else

                            Dim fileLog As System.IO.StreamWriter
                            fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                            fileLog.WriteLine("La riga di dettaglio numero " + riga.NumRiga.ToString + " della spedizione con keyId " + Fattura.keyId.ToString + " non è stata esportata correttamente all' interno del file Fatt_rig il " + Now.ToString)
                            fileLog.Close()
                            mEsporta.EsportaFatturaDiErroreRiga(riga.Ordine, txtFattureRig.Text, riga)

                        End If

                        counterRig += 1

                    Next

                Else
                    Dim fileLog As System.IO.StreamWriter
                    fileLog = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
                    fileLog.WriteLine("La spedizione con keyId " + Fattura.keyId.ToString + " non è stata esportata correttamente all'interno del file Fatt_Gen il " + Now.ToString)
                    fileLog.Close()
                End If

            Next

            For Each Fattura In Fatture

                If listSuccessGem.Contains(Fattura.keyId) Then
                    mModel.FileGeneratoTrue(Fattura.keyId, mConnectionString) '******* mette il bit esportato = true 
                End If

            Next

            'Dim percorsoEseguibile As String = Application.StartupPath
            'Dim percorsoScriptFattGen As String = Path.Combine(percorsoEseguibile, "scriptFattgen.prg")
            'cdxcre.EseguiFoxPro(percorsoScriptFattGen, "C:\Program Files (x86)\Microsoft Visual FoxPro 9\vfp9.exe")
            'cdxcre.EseguiFoxPro(percorsoScriptFattRig, "C:\Program Files (x86)\Microsoft Visual FoxPro 9\vfp9.exe")

            itemTrasmettitoreGen.Text = "Esportazione di " + listSuccessGem.Count.ToString + "/" + Fatture.Count.ToString + " Fatture generali completate il " + Now.ToString
            lvwTrasmettitore.Items.Add(itemTrasmettitoreGen)


            itemTrasmettitorerig.Text = "Esportazione di " + listSuccessRig.Count.ToString + "/" + counterRig.ToString + " Fatture righe completate il " + Now.ToString
            lvwTrasmettitore.Items.Add(itemTrasmettitorerig)

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(itemTrasmettitoreGen.Text) 'Log
            fileLo.WriteLine(itemTrasmettitorerig.Text) 'Log

            fileLo.Close()

        Catch ex As Exception

            Dim itemTrasmettitore As New ListViewItem
            itemTrasmettitore.Text = "Esportazione spedizioni non riuscita : " + ex.Message + " il " + Now.ToString

            Dim fileLo As System.IO.StreamWriter
            fileLo = My.Computer.FileSystem.OpenTextFileWriter(mPathAttualeLog, True)
            fileLo.WriteLine(itemTrasmettitore.Text)   'Log
            fileLo.Close()

            lvwTrasmettitore.Items.Add(itemTrasmettitore)

        End Try

    End Sub


    Public Function ControllaDataFile(filePath As String, lastData As DateTime, nomeXml As String)

        If System.IO.File.Exists(filePath) Then
            Dim fileDate As DateTime = System.IO.File.GetLastWriteTime(filePath)
            'lastData = fileDate

            If nomeXml = "DataArticoli" Then
                mDataArticoli = fileDate
            ElseIf nomeXml = "DataCliForn" Then
                mDataClientiFornitori = fileDate
            ElseIf nomeXml = "DataDestinazioni" Then
                mDataDestinazioniClienti = fileDate
            End If

            If fileDate.ToString("yyyy-MM-dd HH:mm:ss") = lastData.ToString("yyyy-MM-dd HH:mm:ss") Then  'ignora i millisecondi altrimenti non funziona

                Return False

            Else

                printDataOnXML(nomeXml, fileDate)
                Return True

            End If

        Else
            Console.WriteLine("Il files non esiste.")
        End If

    End Function


    Public Sub printDataOnXML(nomeXml As String, fileDate As DateTime)

        Dim exePath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim doc As New XmlDocument()

        Dim configPath As String = Path.Combine(exePath, "ImportaDBF.exe.config")
        doc.Load(configPath)

        ' Modifica data nel file xml
        Dim dateNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='" + nomeXml + "']")  'Aggiorna la data nel file xml
        If dateNode IsNot Nothing Then
            dateNode.Attributes("value").Value = fileDate
        End If

        doc.Save(configPath)

        ' UpdateXmlFile("C:\_PROGETTI\VBNET\EsportaDBF\EsportaDBF\App.config", nomeXml, fileDate) 'solo per debug
    End Sub

    'Private Sub UpdateXmlFile(filePath As String, nomeXml As String, fileDate As DateTime)  'solo per debug

    '    Dim doc As New XmlDocument()
    '    doc.Load(filePath)

    '    Dim dateNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='" & nomeXml & "']")
    '    If dateNode IsNot Nothing Then
    '        dateNode.Attributes("value").Value = fileDate.ToString("yyyy-MM-ddTHH:mm:ss") ' Formattazione standard XML
    '    End If

    '    doc.Save(filePath)

    'End Sub

    'Public Sub aggiornaKeyIdSpedizioniXML(keyId As Integer)

    '    Dim exePath As String = AppDomain.CurrentDomain.BaseDirectory
    '    Dim doc As New XmlDocument()

    '    Dim configPath As String = Path.Combine(exePath, "ImportaDBF.exe.config")
    '    doc.Load(configPath)

    '    Dim keyIdnodo As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='UltimoKeyIdSpedizione']")  'Aggiorna la data nel file xml

    '    If keyIdnodo IsNot Nothing Then
    '        keyIdnodo.Attributes("value").Value = keyId.ToString ' Formattazione standard XML
    '    End If
    '    doc.Save(configPath)
    'End Sub

    Private Sub btnSalvaImpostazioni_Click(sender As Object, e As EventArgs) Handles btnSalvaImpostazioni.Click

        '    UpdateAppConfig() '                                                         solo per debug
        Dim exePath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim doc As New XmlDocument()

        Dim configPath As String = Path.Combine(exePath, "ImportaDBF.exe.config")
        doc.Load(configPath)

        ' Modifica Articoli
        Dim ArticoliNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathArticoli']")
        If ArticoliNode IsNot Nothing Then
            ArticoliNode.Attributes("value").Value = txtArticoli.Text
        End If

        ' Modifica Clienti/Fornitori
        Dim ClientiFornitoriNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathClientiFornitori']")
        If ClientiFornitoriNode IsNot Nothing Then
            ClientiFornitoriNode.Attributes("value").Value = txtClientiFornitori.Text
        End If

        'destinazioni
        Dim destinazioniClienti As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathDestinazioni']")
        If destinazioniClienti IsNot Nothing Then
            destinazioniClienti.Attributes("value").Value = txtDestinazioniClienti.Text
        End If

        'modifica per le fatture 
        Dim FattureGenNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathFattGen']")
        If FattureGenNode IsNot Nothing Then
            FattureGenNode.Attributes("value").Value = txtFattureGen.Text
        End If
        Dim FattureRigNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathFattRig']")
        If FattureRigNode IsNot Nothing Then
            FattureRigNode.Attributes("value").Value = txtFattureRig.Text
        End If

        ' Modifica il tempo
        Dim parts() As String = txtTempo.Text.Split("."c)
        Dim minuti As Integer = 0
        Dim secondi As Integer = 0

        Dim partsfatt() As String = txtTempoAggiornamentoFatt.Text.Split("."c)
        Dim minutifatt As Integer = 0
        Dim secondifatt As Integer = 0

        If parts.Length >= 1 Then minuti = CInt(parts(0))
        If parts.Length = 2 Then secondi = CInt(parts(1))

        If partsfatt.Length >= 1 Then minutifatt = CInt(partsfatt(0))
        If partsfatt.Length = 2 Then secondifatt = CInt(partsfatt(1))

        Dim tempoTotale As Integer = (minuti * 60) + secondi
        Dim tempoTotaleFatt As Integer = (minutifatt * 60) + secondifatt

        Dim TempoNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='Time']")
        If TempoNode IsNot Nothing Then
            TempoNode.Attributes("value").Value = tempoTotale.ToString()
        End If

        Dim TimeFatt As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='TimeFatt']")
        If TimeFatt IsNot Nothing Then
            TimeFatt.Attributes("value").Value = tempoTotaleFatt.ToString()
        End If

        Dim TempoEliminazioneFile As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='GiorniPerEliminazioneFile']")
        If TempoEliminazioneFile IsNot Nothing Then
            TempoEliminazioneFile.Attributes("value").Value = txtTempoEliminazioneFile.Text
        End If

        ' Salva le modifiche
        doc.Save(configPath)

        exePath = Assembly.GetExecutingAssembly().Location
        Process.Start(exePath) ' Avvia una nuova istanza
        Environment.Exit(0)     ' Termina quella attuale


    End Sub


    'Public Sub UpdateAppConfig()           'solo per debug


    '    Dim exePath As String = AppDomain.CurrentDomain.BaseDirectory
    '    Dim doc As New XmlDocument()

    '    Dim configPath As String = "C:\_PROGETTI\VBNET\EsportaDBF\EsportaDBF\app.config"
    '    doc.Load(configPath)

    '    ' Modifica Articoli
    '    Dim ArticoliNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathArticoli']")
    '    If ArticoliNode IsNot Nothing Then
    '        ArticoliNode.Attributes("value").Value = txtArticoli.Text
    '    End If

    '    ' Modifica Clienti/Fornitori
    '    Dim ClientiFornitoriNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathClientiFornitori']")
    '    If ClientiFornitoriNode IsNot Nothing Then
    '        ClientiFornitoriNode.Attributes("value").Value = txtClientiFornitori.Text
    '    End If

    '    'destinazioni
    '    Dim destinazioniClienti As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathDestinazioni']")
    '    If destinazioniClienti IsNot Nothing Then
    '        destinazioniClienti.Attributes("value").Value = txtDestinazioniClienti.Text
    '    End If

    '    'modifica per le fatture 
    '    Dim FattureGenNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathFattGen']")
    '    If FattureGenNode IsNot Nothing Then
    '        FattureGenNode.Attributes("value").Value = txtFattureGen.Text
    '    End If
    '    Dim FattureRigNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='PathFattRig']")
    '    If FattureRigNode IsNot Nothing Then
    '        FattureRigNode.Attributes("value").Value = txtFattureRig.Text
    '    End If

    '    ' Modifica il tempo
    '    Dim parts() As String = txtTempo.Text.Split("."c)
    '    Dim minuti As Integer = 0
    '    Dim secondi As Integer = 0

    '    Dim partsfatt() As String = txtTempoAggiornamentoFatt.Text.Split("."c)
    '    Dim minutifatt As Integer = 0
    '    Dim secondifatt As Integer = 0

    '    If parts.Length >= 1 Then minuti = CInt(parts(0))
    '    If parts.Length = 2 Then secondi = CInt(parts(1))

    '    If partsfatt.Length >= 1 Then minutifatt = CInt(partsfatt(0))
    '    If partsfatt.Length = 2 Then secondifatt = CInt(partsfatt(1))

    '    Dim tempoTotale As Integer = (minuti * 60) + secondi
    '    Dim tempoTotaleFatt As Integer = (minutifatt * 60) + secondifatt

    '    Dim TempoNode As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='Time']")
    '    If TempoNode IsNot Nothing Then
    '        TempoNode.Attributes("value").Value = tempoTotale.ToString()
    '    End If

    '    Dim TimeFatt As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='TimeFatt']")
    '    If TimeFatt IsNot Nothing Then
    '        TimeFatt.Attributes("value").Value = tempoTotaleFatt.ToString()
    '    End If

    '    Dim TempoEliminazioneFile As XmlNode = doc.SelectSingleNode("/configuration/appSettings/add[@key='GiorniPerEliminazioneFile']")
    '    If TempoEliminazioneFile IsNot Nothing Then
    '        TempoEliminazioneFile.Attributes("value").Value = txtTempoEliminazioneFile.Text
    '    End If

    '    ' Salva le modifiche
    '    doc.Save(configPath)

    'End Sub



    Private Sub TimerRefreshImportazione_Tick(sender As Object, e As EventArgs) Handles TimerRefreshImportazione.Tick
        articoli(False)
        ClientiFornitori(False)
        DestinazioniClienti(False)
    End Sub


    Private Sub TimerControllerLog_Tick(sender As Object, e As EventArgs) Handles TimerControllerLog.Tick

        If Date.Today > dataUltimoLog Then
            CreaNuovoFileLog()
            dataUltimoLog = Date.Today
            EliminaVecchiFile()
        End If

    End Sub


    Private Sub CreaNuovoFileLog()

        Dim nomeFile As String = $"log_{Date.Today:yyyy-MM-dd}.txt"
        Dim percorsoCompleto As String = Path.Combine(mPathCartellaLog, nomeFile)

        Using sw As New StreamWriter(percorsoCompleto, False, Encoding.UTF8)

            sw.WriteLine($"Log creato il {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            sw.WriteLine("Inizio log giornaliero.")

        End Using

        mPathAttualeLog = Path.Combine(Application.StartupPath + "/Log", nomeFile)

    End Sub


    Public Sub EliminaVecchiFile()

        If Directory.Exists(mPathCartellaLog) Then
            For Each files As String In Directory.GetFiles(mPathCartellaLog)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(mPathCartellaLog, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If

        Dim PathCliForn = Application.StartupPath + "\Backup\cli_forn"

        If Directory.Exists(PathCliForn) Then
            For Each files As String In Directory.GetFiles(PathCliForn)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(PathCliForn, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If

        Dim Pathdeanarti = Application.StartupPath + "\Backup\deanarti"

        If Directory.Exists(Pathdeanarti) Then
            For Each files As String In Directory.GetFiles(Pathdeanarti)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(Pathdeanarti, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If

        Dim Pathfattgen = Application.StartupPath + "\Backup\fattgen"

        If Directory.Exists(Pathfattgen) Then
            For Each files As String In Directory.GetFiles(Pathfattgen)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(Pathfattgen, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If

        Dim Pathfattrig = Application.StartupPath + "\Backup\fattrig"

        If Directory.Exists(Pathfattrig) Then
            For Each files As String In Directory.GetFiles(Pathfattrig)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(Pathfattrig, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If

        Dim Pathfilcli = Application.StartupPath + "\Backup\filcli"

        If Directory.Exists(Pathfilcli) Then
            For Each files As String In Directory.GetFiles(Pathfilcli)
                Dim dataFile As Date = File.GetCreationTime(files)

                If dataFile < Date.Today.AddDays(-giorniEliminazioneFile) Then
                    Try
                        File.Delete(files)
                    Catch ex As Exception

                        File.AppendAllText(Path.Combine(Pathfilcli, "error_log.txt"),
                                           $"{DateTime.Now}: Errore eliminazione {files} - {ex.Message}{Environment.NewLine}")
                    End Try
                End If
            Next
        End If


    End Sub

    Private Sub txtTempo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtTempo.KeyPress
        If e.KeyChar = ","c Then
            e.Handled = True ' blocca la virgola
        End If

        Dim KeyAscii As Short = Asc(e.KeyChar)


        If KeyAscii < 48 And KeyAscii <> 24 And KeyAscii <> 8 Then
            KeyAscii = 0
        ElseIf KeyAscii > 57 Then
            KeyAscii = 0
        End If

        If e.KeyChar = "0" And txtTempo.TextLength = 0 And KeyAscii > 57 And KeyAscii < 48 And KeyAscii <> 24 And KeyAscii <> 8 Then

            KeyAscii = 0

        End If

        e.KeyChar = Chr(KeyAscii)

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles BtnRicaricaFatture.Click

        Fatture()

    End Sub

    Private Sub btnSvuotaTrasmettitore_Click(sender As Object, e As EventArgs) Handles btnSvuotaTrasmettitore.Click
        lvwTrasmettitore.Items.Clear()
    End Sub

    Private Sub txtTempoEliminazioneFile_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtTempoEliminazioneFile.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)


        If KeyAscii < 48 And KeyAscii <> 24 And KeyAscii <> 8 Then
            KeyAscii = 0
        ElseIf KeyAscii > 57 Then
            KeyAscii = 0
        End If

        If e.KeyChar = "0" And txtTempo.TextLength = 0 And KeyAscii > 57 And KeyAscii < 48 And KeyAscii <> 24 And KeyAscii <> 8 Then

            KeyAscii = 0

        End If

        e.KeyChar = Chr(KeyAscii)
    End Sub

    Private Sub TimerRefreshFatture_Tick(sender As Object, e As EventArgs) Handles TimerRefreshFatture.Tick
        Fatture()
    End Sub
End Class
