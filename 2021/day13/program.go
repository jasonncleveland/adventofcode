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
	coordinates, instructions := ParseInput(lines)

	for _, instruction := range instructions {
		uniqueCoordinates := map[point]bool{}
		for index, coordinate := range coordinates {
			if instruction.x > 0 && coordinate.x > instruction.x {
				delta := coordinate.x - instruction.x
				newCoordinate := point{instruction.x - delta, coordinate.y}
				uniqueCoordinates[newCoordinate] = true
				coordinates[index] = newCoordinate
			} else if instruction.y > 0 && coordinate.y > instruction.y {
				delta := coordinate.y - instruction.y
				newCoordinate := point{coordinate.x, instruction.y - delta}
				uniqueCoordinates[newCoordinate] = true
				coordinates[index] = newCoordinate
			} else {
				uniqueCoordinates[coordinate] = true
			}
		}
		return int64(len(uniqueCoordinates))
	}
	return -1
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) ([]point, []point) {
	var coordinates []point
	var instructions []point

	index := 0
	// Parse coordinates
	for ; ; index++ {
		line := lines[index]
		if len(line) == 0 {
			break
		}
		lineParts := bytes.Split(line, []byte(","))
		x := ParseNumber(lineParts[0])
		y := ParseNumber(lineParts[1])
		coordinates = append(coordinates, point{x, y})
	}

	index++

	// Parse instructions
	for ; index < len(lines); index++ {
		line := lines[index]
		lineParts := bytes.Split(line, []byte(" "))
		instructionParts := bytes.Split(lineParts[2], []byte("="))
		axis := instructionParts[0]
		value := ParseNumber(instructionParts[1])
		var x, y int64
		switch axis[0] {
		case byte('x'):
			x = value
		case byte('y'):
			y = value
		}
		instructions = append(instructions, point{x, y})
	}

	return coordinates, instructions
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

func ParseNumber(bytes []byte) int64 {
	magnitude := int64(1)
	number := int64(0)
	for index := range bytes {
		number += int64(bytes[len(bytes)-1-index]-byte('0')) * magnitude
		magnitude *= 10
	}
	return number
}

type point struct {
	x int64
	y int64
}
