CLEAR
SET SAFETY OFF

USE "C:\Users\POS5\Desktop\Connettore MagTrace\Fatture\Demo\fattgen.dbf"  

* Elimina indici specifici 
DELETE TAG codice
DELETE TAG descriz
DELETE TAG totale
DELETE TAG totale_e
DELETE TAG data_ordi
DELETE TAG data_nr

* Ricrea gli indici
INDEX ON codice TAG codice
INDEX ON descriz TAG descriz
INDEX ON totale TAG totale
INDEX ON totale_e TAG totale_e
INDEX ON data_ordi TAG data_ordi
INDEX ON ALLTRIM(STR(YEAR(data_ordi))) + STR(nr_ordi) TAG data_nr

USE
QUIT
