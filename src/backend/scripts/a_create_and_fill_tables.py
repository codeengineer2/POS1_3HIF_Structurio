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
        CREATE TABLE IF NOT EXISTS users (
            uid SERIAL PRIMARY KEY,
            firstname VARCHAR(100) NOT NULL,
            lastname VARCHAR(100) NOT NULL,
            email VARCHAR(150) UNIQUE NOT NULL,
            password_hash TEXT NOT NULL,
            birthdate DATE NOT NULL
        );

        CREATE TABLE IF NOT EXISTS projects (
            pid SERIAL PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            description TEXT,
            color VARCHAR(20),
            owner_uid INT REFERENCES users(uid) ON DELETE SET NULL
        );

        CREATE TABLE IF NOT EXISTS boards (
            bid SERIAL PRIMARY KEY,
            pid INT UNIQUE NOT NULL REFERENCES projects(pid) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS columns (
            cid SERIAL PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            bid INT NOT NULL REFERENCES boards(bid) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS issues (
            iid SERIAL PRIMARY KEY,
            description TEXT NOT NULL,
            cid INT NOT NULL REFERENCES columns(cid) ON DELETE CASCADE
        );

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
        cur.execute("""
            INSERT INTO users (firstname, lastname, email, password_hash, birthdate) VALUES (%s, %s, %s, %s, %s)
        """, (f"User{i}", f"Test{i}", f"user{i}@example.com", f"hashed_pw_{i}", date(2000+i, 1, 1)))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO projects (name, description, color, owner_uid) VALUES (%s, %s, %s, %s)
        """, (f"Project{i}", f"Description for project {i}", "#FF5733", i))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO boards (pid) VALUES (%s)
        """, (i,))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO columns (name, bid) VALUES (%s, %s)
        """, (f"Column{i}", i))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO issues (description, cid) VALUES (%s, %s)
        """, (f"Issue description {i}", i))

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