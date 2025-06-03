CLEAR
SET SAFETY OFF

USE "C:\Users\POS5\Desktop\Connettore MagTrace\Fatture\Demo\fattrig.dbf" EXCLUSIVE

* Ricrea direttamente gli indici (sovrascrive se già esistono)
INDEX ON cod TAG cod
INDEX ON anno + STR(ordine) + cf TAG annocf

USE
QUIT