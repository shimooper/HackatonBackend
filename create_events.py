#import csv
import requests
import pandas as pd
import time


EVENTS_CSV_FILE_PATH = r"C:\repos\HackatonBackend\Hackacton Events Data.csv"
API_ENDPOINT = "https://aicalendarbackend.azurewebsites.net/api/events"


def main():
    df = pd.read_csv(EVENTS_CSV_FILE_PATH)
    for i in range(len(df)):
        event_data = {
            "Name": df.iloc[i, 0],
            "Description":  df.iloc[i, 1],
            "Location":  df.iloc[i, 2],
            "StarTime":  df.iloc[i, 3],
            "EndTime":  df.iloc[i, 4],
            "Price":  float(df.iloc[i, 5]),
            "Tags":  df.iloc[i, 6],
            "Language":  df.iloc[i, 7]
        }

        r = requests.post(API_ENDPOINT, json=event_data)
        print(r.status_code)
        time.sleep(1)


if __name__ == "__main__":
    main()