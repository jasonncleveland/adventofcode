package day13

import (
	"bytes"
	"fmt"
	"strings"
	"time"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFileLines(fileName)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(lines)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(lines)
	fmt.Printf("Part 2: %s (%s)\n", part2, time.Since(start))
}

func Part1(lines [][]byte) int64 {
	var coordinates, instructions []point = ParseInput(lines)

	// Only process the first instruction
	var uniqueCoordinates map[point]bool = GetUniqueCoordinates(coordinates, instructions[:1])

	return int64(len(uniqueCoordinates))
}

func Part2(lines [][]byte) string {
	var coordinates, instructions []point = ParseInput(lines)

	var uniqueCoordinates map[point]bool = GetUniqueCoordinates(coordinates, instructions)

	// Calculate the boundaries of the output text
	var maxX, maxY int64
	for coordinate := range uniqueCoordinates {
		if coordinate.x > maxX {
			maxX = coordinate.x
		}
		if coordinate.y > maxY {
			maxY = coordinate.y
		}
	}
	width := maxX + 1
	height := maxY + 1

	// Build the output text string
	var stringBuilder strings.Builder
	stringBuilder.Grow(int(width * height))
	stringBuilder.WriteRune('\n')
	for row := range height {
		for column := range width {
			if uniqueCoordinates[point{column, row}] {
				// Print the rune purple and bold (\033[1;35m) then reset (\033[0m)
				// stringBuilder.WriteString("\033[1;35m")
				stringBuilder.WriteRune('█')
				// stringBuilder.WriteString("\033[0m")
			} else {
				stringBuilder.WriteRune(' ')
			}
		}
		stringBuilder.WriteRune('\n')
	}
	return stringBuilder.String()
}

func GetUniqueCoordinates(coordinates []point, instructions []point) map[point]bool {
	var uniqueCoordinates map[point]bool

	for _, instruction := range instructions {
		uniqueCoordinates = map[point]bool{}
		for index, coordinate := range coordinates {
			newCoordinate := coordinate
			if instruction.x > 0 && coordinate.x > instruction.x {
				delta := coordinate.x - instruction.x
				newCoordinate = point{instruction.x - delta, coordinate.y}
				coordinates[index] = newCoordinate
			} else if instruction.y > 0 && coordinate.y > instruction.y {
				delta := coordinate.y - instruction.y
				newCoordinate = point{coordinate.x, instruction.y - delta}
				coordinates[index] = newCoordinate
			}
			uniqueCoordinates[newCoordinate] = true
		}
	}

	return uniqueCoordinates
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
