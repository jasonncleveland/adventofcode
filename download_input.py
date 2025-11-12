#!/usr/bin/python3

import requests
import sys

from pathlib import Path

if __name__ == '__main__':
    if len(sys.argv) < 3:
        raise ValueError("must provide year and day to download as arguments")

    year = sys.argv[1]
    day = sys.argv[2]
    print(f'Received year {year} and day {day} as arguments')

    file_path = f'inputs/{year}/day/{day}/input'
    output_file = Path(file_path)
    if output_file.exists():
        print(f'Input already exists for year {year} day {day}')
        raise SystemExit()

    print('Reading session token from file')
    with open('session', 'r') as f:
        # Remove BOM if found
        session = f.read().replace('\ufeff', '')

    url = f'https://adventofcode.com/{year}/day/{day}/input'
    headers = {
        'Cookie': f'session={session}'
    }

    print(f'Downloading input data for year {year} day {day} from {url}')
    response = requests.get(url, headers=headers)
    if response.status_code != 200:
        print('request failed')
        print(response.text)

    print(f'Writing input data for year {year} day {day} to {file_path}')
    output_file.parent.mkdir(exist_ok=True, parents=True)
    output_file.write_text(response.text)
