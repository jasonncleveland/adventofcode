package day15

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
	var grid utils.IntGrid = utils.ParseIntGrid(lines)

	return grid.Djikstra(utils.Coordinate{Row: 0, Column: 0}, utils.Coordinate{Row: len(grid) - 1, Column: len(grid) - 1})
}

func Part2(lines [][]byte) int64 {
	return -1
}
