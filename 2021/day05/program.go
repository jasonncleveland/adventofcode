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
	pointMap := make(map[string]int64)
	for _, line := range ParseInput(lines) {
		if line.start.x != line.end.x && line.start.y != line.end.y {
			// Ignore diagonal lines
			continue
		}

		if line.start.x == line.end.x {
			for y := line.start.y; y <= line.end.y; y++ {
				pointMap[fmt.Sprintf("%d,%d", line.start.x, y)] += 1
			}
			for y := line.end.y; y <= line.start.y; y++ {
				pointMap[fmt.Sprintf("%d,%d", line.start.x, y)] += 1
			}
		} else if line.start.y == line.end.y {
			for x := line.start.x; x <= line.end.x; x++ {
				pointMap[fmt.Sprintf("%d,%d", x, line.start.y)] += 1
			}
			for x := line.end.x; x <= line.start.x; x++ {
				pointMap[fmt.Sprintf("%d,%d", x, line.start.y)] += 1
			}
		}
	}
	total := int64(0)
	for _, count := range pointMap {
		if count > 1 {
			total++
		}
	}
	return total
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) []line {
	var data []line

	for _, item := range lines {
		points := bytes.Split(item, []byte(" -> "))

		startPoint := bytes.Split(points[0], []byte(","))
		startX := ParseNumber(startPoint[0])
		startY := ParseNumber(startPoint[1])

		endPoint := bytes.Split(points[1], []byte(","))
		endX := ParseNumber(endPoint[0])
		endY := ParseNumber(endPoint[1])

		data = append(data, line{
			point{startX, startY},
			point{endX, endY},
		})
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

type line struct {
	start point
	end   point
}
