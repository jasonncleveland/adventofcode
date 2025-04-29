package day18

import (
	"fmt"
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
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(lines [][]byte) int64 {
	var snailNumbers []*SnailNumber = ParseInput(lines)

	var total *SnailNumber = snailNumbers[0]
	for i := 1; i < len(snailNumbers); i++ {
		snailNumber := snailNumbers[i]
		total = Add(total, snailNumber)
	}
	return total.Magnitude()
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) []*SnailNumber {
	data := make([]*SnailNumber, len(lines))

	for i, line := range lines {
		data[i] = Parse(line)
	}

	return data
}
