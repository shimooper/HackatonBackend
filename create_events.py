import csv
import requests


EVENTS_CSV_FILE_PATH = r"E:\repos\HackatonBackend\Events.csv"
API_ENDPOINT = "https://aicalendarbackend.azurewebsites.net/api/events"


def create_event(row):
    event_data = {
        "Name": row[0],
        "Description": row[1],
        "Location": row[2],
        "StarTime": row[3],
        "EndTime": row[4],
        "Price": row[5],
        "Tags": row[6],
        "Language": row[7]
    }

    r = requests.post(API_ENDPOINT, json=event_data)
    print(r.status_code)


def main():
    with open(EVENTS_CSV_FILE_PATH, encoding="ISO-8859-8") as csvfile:
        reader = csv.reader(csvfile)
        for row in reader:
            if not row:
                continue

            create_event(row)


if __name__ == "__main__":
    main()