package main

import (
	"bytes"
	"container/list"
	"fmt"
	"math"
	"os"
	"slices"
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
	grid := ParseInput(lines)

	total := int64(0)
	for row := range grid {
		for column := range grid[row] {
			if IsLowPoint(grid, row, column) {
				total += 1 + grid[row][column]
			}
		}
	}
	return total
}

func Part2(lines [][]byte) int64 {
	grid := ParseInput(lines)

	visited := make(map[coordinate]bool)

	var basinSizes []int64
	for row := range grid {
		for column := range grid[row] {
			position := coordinate{row, column}
			if grid[row][column] != 9 && !visited[position] {
				basinSizes = append(basinSizes, FindBasin(grid, position, visited))
			}
		}
	}

	slices.Sort(basinSizes)
	slices.Reverse(basinSizes)

	return basinSizes[0] * basinSizes[1] * basinSizes[2]
}

func ParseInput(lines [][]byte) [][]int64 {
	var data [][]int64

	for _, line := range lines {
		var bytes []int64
		for _, bit := range line {
			bytes = append(bytes, int64(bit-byte('0')))
		}
		data = append(data, bytes)
	}

	return data
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

func IsValid(grid [][]int64, row int, column int) bool {
	return row >= 0 && row < len(grid) && column >= 0 && column < len(grid[row])
}

func IsLowPoint(grid [][]int64, row int, column int) bool {
	var min int64 = math.MaxInt64
	if IsValid(grid, row, column-1) && grid[row][column-1] < min {
		min = grid[row][column-1]
	}
	if IsValid(grid, row, column+1) && grid[row][column+1] < min {
		min = grid[row][column+1]
	}
	if IsValid(grid, row-1, column) && grid[row-1][column] < min {
		min = grid[row-1][column]
	}
	if IsValid(grid, row+1, column) && grid[row+1][column] < min {
		min = grid[row+1][column]
	}
	return grid[row][column] < min
}

func FindBasin(grid [][]int64, position coordinate, visited map[coordinate]bool) int64 {
	queue := list.New()

	queue.PushBack(position)
	visited[position] = true

	total := int64(0)
	for queue.Len() > 0 {
		point := queue.Front().Value.(coordinate)
		queue.Remove(queue.Front())

		row := point.row
		column := point.column
		total += 1

		if !visited[coordinate{row, column - 1}] && IsValid(grid, row, column-1) && grid[row][column-1] != 9 {
			visited[coordinate{row, column - 1}] = true
			queue.PushBack(coordinate{row, column - 1})
		}
		if !visited[coordinate{row, column + 1}] && IsValid(grid, row, column+1) && grid[row][column+1] != 9 {
			visited[coordinate{row, column + 1}] = true
			queue.PushBack(coordinate{row, column + 1})
		}
		if !visited[coordinate{row - 1, column}] && IsValid(grid, row-1, column) && grid[row-1][column] != 9 {
			visited[coordinate{row - 1, column}] = true
			queue.PushBack(coordinate{row - 1, column})
		}
		if !visited[coordinate{row + 1, column}] && IsValid(grid, row+1, column) && grid[row+1][column] != 9 {
			visited[coordinate{row + 1, column}] = true
			queue.PushBack(coordinate{row + 1, column})
		}
	}

	return total
}

type coordinate struct {
	row    int
	column int
}
