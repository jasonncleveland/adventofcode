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
	instructions := ParseInput(lines)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(instructions)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))
}

func Part1(instructions []instruction) int64 {
	horizontalPosition := int64(0)
	depth := int64(0)

	for _, instruction := range instructions {
		switch instruction.command {
		case "forward":
			horizontalPosition += instruction.distance
		case "down":
			depth += instruction.distance
		case "up":
			depth -= instruction.distance
		}
	}

	return horizontalPosition * depth
}

func ParseInput(lines [][]byte) []instruction {
	var instructions []instruction

	for _, line := range lines {
		lineParts := bytes.Split(line, []byte(" "))
		instruction := instruction{
			command:  string(lineParts[0]),
			distance: int64(lineParts[1][0] - byte('0')),
		}
		instructions = append(instructions, instruction)
	}

	return instructions
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

type instruction struct {
	command  string
	distance int64
}
