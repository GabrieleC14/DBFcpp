Public Class Dichiarazioni
End Class

Public Class Fatture_Gen 'spedizione

    Public Property DataOra As DateTime
    Public Property ordine As Integer
    Public Property Indirizzo As String
    Public Property RagioneSociale As String
    Public Property Provincia As String
    Public Property Citta As String
    Public Property Cap As String
    Public Property codiceCliente As String
    Public Property DataConsegna As DateTime
    'Public Property NumDocumento As Integer
    Public Property NumColli As Integer
    Public Property keyId As Integer
    Public Property sezionale As String
    Public Property numDoc As Integer
    Public Property tipo As Integer

End Class

Public Class Fatture_Righe 'spedizione_dettagli

    Public Property Anno As Integer
    Public Property Ordine As Integer
    Public Property NumRiga As Integer
    Public Property CodiceArticolo As String
    Public Property DescrizioneArticolo As String
    Public Property Lotto As String
    Public Property Qta As Double        '11, 4
    Public Property Um As String
    Public Property PrezzoUnit As Double '11, 2
    Public Property Sconto As Double     ' 5, 2
    Public Property Sconto2 As Double    ' 3
    Public Property Sconto3 As Double    ' 3
    Public Property Iva As Integer

End Class

Public Class Articoli

    Public Property CodArticolo As String
    Public Property Descrizione As String
    Public Property IVA As String
    Public Property UM As String
    Public Property Prezzo As String

End Class

Public Class ClientiFornitori

    Public Property Codice As String
    Public Property RagioneSociale As String
    Public Property Indirizzo As String
    Public Property Cap As String
    Public Property Citta As String
    Public Property Provincia As String
    Public Property CodFiscale As String
    Public Property PIva As String
    Public Property Telefono As String
    Public Property CarattereRiconoscitore As String

End Class

Public Class DestinazioniClienti

    Public Property CodiceCliente As String
    Public Property CodiceDestinazione As Integer
    Public Property Indirizzo As String
    Public Property Citta As String
    Public Property cap As String
    Public Property provincia

End Class

Public Class Clienti

    Public Property Codice As String
    Public Property RagioneSociale As String
    Public Property Indirizzo As String
    Public Property Cap As String
    Public Property Citta As String
    Public Property Provincia As String
    Public Property CodFiscale As String
    Public Property PIva As String
    Public Property Telefono As String


    ' Public Property Destinazoni As String 
End Class

Public Class Fornitori

    Public Property Codice As String
    Public Property RagioneSociale As String
    Public Property Indirizzo As String
    Public Property Cap As Integer
    Public Property Citta As String
    Public Property Provincia As String
    Public Property CodFiscale As String
    Public Property PIva As String
    Public Property Telefono As Integer

End Class
