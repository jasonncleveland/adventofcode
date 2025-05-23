package day11

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
	grid := utils.ParseIntGrid(lines)

	flashes := int64(0)
	for range 100 {
		ProcessStep(grid, &flashes)
	}
	return flashes
}

func Part2(lines [][]byte) int64 {
	grid := utils.ParseIntGrid(lines)

	flashes := int64(0)
	for step := int64(1); ; step++ {
		flashesBefore := flashes
		ProcessStep(grid, &flashes)

		if flashes-flashesBefore == 100 {
			// Stop when all octopuses have flashed at the same time
			return step
		}
	}
}

func ProcessStep(grid [][]int64, flashes *int64) {
	// Increase energy level by 1 for each octopus
	for row := range grid {
		for column := range grid[row] {
			grid[row][column] += 1
		}
	}

	// Check for flashes
	for row := range grid {
		for column := range grid[row] {
			if grid[row][column] > 9 {
				grid[row][column] = 0
				FlashRec(grid, row, column, flashes)
			}
		}
	}
}

func IncreaseEnergyLevel(grid [][]int64, row int, column int, flashes *int64) {
	if row < 0 || row >= len(grid) || column < 0 || column >= len(grid[row]) {
		return
	}

	switch grid[row][column] {
	case 9:
		// Flash an octopus if its energy level is 9 and increase energy level of all neighbours
		grid[row][column] = 0
		FlashRec(grid, row, column, flashes)
	case 0:
		// Do nothing if an octopus has already flashed
		return
	default:
		// Increase energy level of octopus
		grid[row][column] += 1
	}
}

func FlashRec(grid [][]int64, row int, column int, flashes *int64) {
	*flashes++
	// NW
	IncreaseEnergyLevel(grid, row-1, column-1, flashes)
	// N
	IncreaseEnergyLevel(grid, row-1, column, flashes)
	// NE
	IncreaseEnergyLevel(grid, row-1, column+1, flashes)
	// W
	IncreaseEnergyLevel(grid, row, column-1, flashes)
	// E
	IncreaseEnergyLevel(grid, row, column+1, flashes)
	// SW
	IncreaseEnergyLevel(grid, row+1, column-1, flashes)
	// S
	IncreaseEnergyLevel(grid, row+1, column, flashes)
	// SE
	IncreaseEnergyLevel(grid, row+1, column+1, flashes)
}
