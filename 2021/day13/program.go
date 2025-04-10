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
	var coordinates, instructions []utils.Point2D = ParseInput(lines)

	// Only process the first instruction
	var uniqueCoordinates map[utils.Point2D]bool = GetUniqueCoordinates(coordinates, instructions[:1])

	return int64(len(uniqueCoordinates))
}

func Part2(lines [][]byte) string {
	var coordinates, instructions []utils.Point2D = ParseInput(lines)

	var uniqueCoordinates map[utils.Point2D]bool = GetUniqueCoordinates(coordinates, instructions)

	// Calculate the boundaries of the output text
	var maxX, maxY int64
	for coordinate := range uniqueCoordinates {
		if coordinate.X > maxX {
			maxX = coordinate.X
		}
		if coordinate.Y > maxY {
			maxY = coordinate.Y
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
			if uniqueCoordinates[utils.Point2D{X: column, Y: row}] {
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

func GetUniqueCoordinates(coordinates []utils.Point2D, instructions []utils.Point2D) map[utils.Point2D]bool {
	var uniqueCoordinates map[utils.Point2D]bool

	for _, instruction := range instructions {
		uniqueCoordinates = map[utils.Point2D]bool{}
		for index, coordinate := range coordinates {
			newCoordinate := coordinate
			if instruction.X > 0 && coordinate.X > instruction.X {
				delta := coordinate.X - instruction.X
				newCoordinate = utils.Point2D{X: instruction.X - delta, Y: coordinate.Y}
				coordinates[index] = newCoordinate
			} else if instruction.Y > 0 && coordinate.Y > instruction.Y {
				delta := coordinate.Y - instruction.Y
				newCoordinate = utils.Point2D{X: coordinate.X, Y: instruction.Y - delta}
				coordinates[index] = newCoordinate
			}
			uniqueCoordinates[newCoordinate] = true
		}
	}

	return uniqueCoordinates
}

func ParseInput(lines [][]byte) ([]utils.Point2D, []utils.Point2D) {
	var coordinates []utils.Point2D
	var instructions []utils.Point2D

	index := 0
	// Parse coordinates
	for ; ; index++ {
		line := lines[index]
		if len(line) == 0 {
			break
		}
		lineParts := bytes.Split(line, []byte(","))
		x := utils.ParseNumber(lineParts[0])
		y := utils.ParseNumber(lineParts[1])
		coordinates = append(coordinates, utils.Point2D{X: x, Y: y})
	}

	index++

	// Parse instructions
	for ; index < len(lines); index++ {
		line := lines[index]
		lineParts := bytes.Split(line, []byte(" "))
		instructionParts := bytes.Split(lineParts[2], []byte("="))
		axis := instructionParts[0]
		value := utils.ParseNumber(instructionParts[1])
		var x, y int64
		switch axis[0] {
		case byte('x'):
			x = value
		case byte('y'):
			y = value
		}
		instructions = append(instructions, utils.Point2D{X: x, Y: y})
	}

	return coordinates, instructions
}
