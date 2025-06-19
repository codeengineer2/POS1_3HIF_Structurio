import psycopg2
import random
from datetime import date, time, timedelta

conn_str = (
    "postgresql://structure_owner:npg_cEPXthQ49IRm@"
    "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
    "structure?sslmode=require"
)

try:
    conn = psycopg2.connect(conn_str)
    cur = conn.cursor()

    cur.execute("""
        CREATE TABLE IF NOT EXISTS zeitstempel (
            zid SERIAL PRIMARY KEY,
            uid INT NOT NULL REFERENCES users(uid) ON DELETE CASCADE,
            checkin TIME,
            checkout TIME,
            datum_in DATE,
            duration INTERVAL,
            datum_out DATE,
            "user" VARCHAR(100)
        );

        CREATE TABLE IF NOT EXISTS abrechnung (
            aid SERIAL PRIMARY KEY,
            uid INT NOT NULL REFERENCES users(uid) ON DELETE CASCADE,
            pid INT NOT NULL REFERENCES projects(pid) ON DELETE CASCADE,
            name VARCHAR(100),
            date DATE,
            price REAL,
            category VARCHAR(100),
            rechnung VARCHAR(100),
            "user" VARCHAR(100)
        );
    """)

    for i in range(1, 11):
        checkin_time = time(8 + (i % 2), 30)
        checkout_time = time(17, 30)
        duration = timedelta(hours=9 - (i % 3))
        datum = date(2025, 6, 18)
        cur.execute("""
            INSERT INTO zeitstempel (uid, checkin, checkout, datum_in, duration, datum_out, "user")
            VALUES (%s, %s, %s, %s, %s, %s, %s)
        """, (i, checkin_time, checkout_time, datum, duration, datum, f"user{i}"))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO abrechnung (uid, pid, name, date, price, category, rechnung, "user")
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
        """, (
            i,
            i,
            f"Rechnung {i}",
            date(2025, 6, 7 + i),
            round(100 + i * 7.5, 2),
            "Sonstiges",
            f"../../../Rechnung/Datei_{i}.pdf",
            f"user{i}"
        ))

    conn.commit()
    cur.close()
    conn.close()
    print("Tabellen und Testdaten erfolgreich erstellt.")

except Exception as e:
    print("Fehler beim Ausführen des Skripts:")
    print(e)