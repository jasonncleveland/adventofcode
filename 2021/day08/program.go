package main

import (
	"bytes"
	"fmt"
	"os"
	"time"
)

func main() {
	if len(os.Args) < 2 {
		panic("must provide input file name as first command line argument")
	}
	fileName := os.Args[1]

	var start time.Time = time.Now()
	lines := ReadFileLines(fileName)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(lines)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(lines)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}
func Part1(lines [][]byte) int64 {
	entries := ParseInput(lines)

	total := int64(0)
	for _, entry := range entries {
		for _, output := range entry.outputs {
			if len(output) == 2 || len(output) == 4 || len(output) == 3 || len(output) == 7 {
				total++
			}
		}
	}
	return total
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) []entry {
	var entries []entry

	for _, line := range lines {
		lineParts := bytes.Split(line, []byte(" | "))
		inputs := bytes.Split(lineParts[0], []byte(" "))
		outputs := bytes.Split(lineParts[1], []byte(" "))
		entries = append(entries, entry{
			inputs,
			outputs,
		})
	}

	return entries
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

type entry struct {
	inputs  [][]byte
	outputs [][]byte
}
