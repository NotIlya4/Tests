import psycopg2
import time

# PostgreSQL connection parameters
dbname = "test2"
user = "postgres"
password = "pgpass"
host = "84.201.164.118"
port = "5432"  # Default PostgreSQL port is 5432

# SQL query to execute
sql_query = "SELECT * FROM \"SequentialEntities\" WHERE \"Id\" = 1"

try:
    # Connect to PostgreSQL
    conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host, port=port)
    cursor = conn.cursor()

    # Measure time before executing query
    start_time = time.time()

    # Execute SQL query
    cursor.execute(sql_query)

    # Fetch results if needed, in this case, we just want to measure execution time
    # results = cursor.fetchall()

    # Measure time after executing query
    end_time = time.time()

    # Calculate execution time
    execution_time = end_time - start_time

    print("Query executed successfully.")
    print("Execution Time:", execution_time, "seconds")

    # Close cursor and connection
    cursor.close()
    conn.close()

except psycopg2.Error as e:
    print("Error executing SQL query:", e)
