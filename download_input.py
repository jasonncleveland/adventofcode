#!/usr/bin/python3

import requests
import sys

from pathlib import Path


def download_day(session: str, year: str, day: str) -> bool:
    file_path = f'inputs/{year}/day/{day}/input'
    output_file = Path(file_path)
    if output_file.exists():
        print(f'Input already exists for year {year} day {day}')
        return True

    url = f'https://adventofcode.com/{year}/day/{day}/input'
    headers = {
        'Cookie': f'session={session}'
    }

    print(f'Downloading input data for year {year} day {day} from {url}')
    response = requests.get(url, headers=headers)
    if response.status_code != 200:
        print(f'Request failed while downloading data from {url}')
        print(response.text)
        return False

    print(f'Writing input data for year {year} day {day} to {file_path}')
    output_file.parent.mkdir(exist_ok=True, parents=True)
    output_file.write_text(response.text)
    return True


if __name__ == '__main__':
    if len(sys.argv) < 2:
        raise ValueError("must provide year and (optional) day to download as arguments")

    year = sys.argv[1]
    day = None
    if len(sys.argv) > 2:
        day = sys.argv[2]
    print(f'Received year {year} and day {day} as arguments')

    print('Reading session token from file')
    with open('session', 'r') as f:
        # Remove BOM if found
        session = f.read().replace('\ufeff', '')

    if day is not None:
        print(f'Downloading input for year {year} day {day}')
        download_day(session, year, day)
    else:
        print(f'Downloading inputs for all days in year {year}')
        for i in range(25):
            day = i + 1
            result = download_day(session, year, str(day))
            if result is False:
                print(f'Error encountered while downloading year {year} day {day}')
                raise SystemExit
