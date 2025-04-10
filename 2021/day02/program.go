package day02

import (
	"bytes"
	"fmt"
	"time"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFileLines(fileName)
	instructions := ParseInput(lines)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(instructions)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(instructions)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
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

func Part2(instructions []instruction) int64 {
	horizontalPosition := int64(0)
	depth := int64(0)
	aim := int64(0)

	for _, instruction := range instructions {
		switch instruction.command {
		case "forward":
			horizontalPosition += instruction.distance
			depth += aim * instruction.distance
		case "down":
			aim += instruction.distance
		case "up":
			aim -= instruction.distance
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

type instruction struct {
	command  string
	distance int64
}
