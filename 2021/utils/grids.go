package utils

import (
	"container/heap"
	"fmt"
)

type Coordinate struct {
	Row    int
	Column int
}

type IntRow []int64

type IntGrid [][]int64

func ParseIntGrid(lines [][]byte) IntGrid {
	var grid IntGrid

	for _, line := range lines {
		var bytes []int64
		for _, bit := range line {
			bytes = append(bytes, int64(bit-byte('0')))
		}
		grid = append(grid, bytes)
	}

	return grid
}

func (grid IntGrid) IsValid(row int, column int) bool {
	return row >= 0 && row < len(grid) && column >= 0 && column < len(grid[row])
}

func (grid IntGrid) At(row int, column int) int64 {
	if !grid.IsValid(row, column) {
		panic(fmt.Sprintf("coordinate {%d,%d} out of range", row, column))
	}
	return grid[row][column]
}

func (grid IntGrid) Print(highlightValue int64) {
	for row := range grid {
		for column := range grid[row] {
			if grid[row][column] == highlightValue {
				// Print the number red (\033[31) and bold (\033[1m) then reset (\033[0m)
				fmt.Printf("\033[1m\033[31m%v\033[0m", grid[row][column])
			} else {
				fmt.Printf("%v", grid[row][column])
			}
		}
		fmt.Printf("\n")
	}
}

func (grid IntGrid) Dijkstra(start Coordinate, end Coordinate) int64 {
	visited := make([][]bool, len(grid))
	for row := range grid {
		visited[row] = make([]bool, len(grid[row]))
	}

	pq := PriorityQueue{&QueueItem{Coordinate: start, Priority: 0}}
	heap.Init(&pq)

	for pq.Len() > 0 {
		item := heap.Pop(&pq).(*QueueItem)

		row := item.Row
		column := item.Column

		if item.Coordinate == end {
			return item.Priority
		}

		if grid.IsValid(row, column-1) && !visited[row][column-1] {
			visited[row][column-1] = true
			heap.Push(&pq, &QueueItem{Coordinate: Coordinate{Row: row, Column: column - 1}, Priority: item.Priority + grid.At(row, column-1)})
		}
		if grid.IsValid(row, column+1) && !visited[row][column+1] {
			visited[row][column+1] = true
			heap.Push(&pq, &QueueItem{Coordinate: Coordinate{Row: row, Column: column + 1}, Priority: item.Priority + grid.At(row, column+1)})
		}
		if grid.IsValid(row-1, column) && !visited[row-1][column] {
			visited[row-1][column] = true
			heap.Push(&pq, &QueueItem{Coordinate: Coordinate{Row: row - 1, Column: column}, Priority: item.Priority + grid.At(row-1, column)})
		}
		if grid.IsValid(row+1, column) && !visited[row+1][column] {
			visited[row+1][column] = true
			heap.Push(&pq, &QueueItem{Coordinate: Coordinate{Row: row + 1, Column: column}, Priority: item.Priority + grid.At(row+1, column)})
		}
	}

	return -1
}
