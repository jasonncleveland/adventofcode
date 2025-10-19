package utils

import (
	"bytes"
	"os"
)

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

func ReadFile(fileName string) []byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}

	return data
}
