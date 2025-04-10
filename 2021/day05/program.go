package day05

import (
	"bytes"
	"fmt"
	"math"
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
	pointMap := make(map[string]int64)
	for _, line := range ParseInput(lines) {
		if line.Start.X != line.End.X && line.Start.Y != line.End.Y {
			// Ignore diagonal lines
			continue
		}

		if line.Start.X == line.End.X {
			for y := line.Start.Y; y <= line.End.Y; y++ {
				pointMap[fmt.Sprintf("%d,%d", line.Start.X, y)] += 1
			}
			for y := line.End.Y; y <= line.Start.Y; y++ {
				pointMap[fmt.Sprintf("%d,%d", line.Start.X, y)] += 1
			}
		} else if line.Start.Y == line.End.Y {
			for x := line.Start.X; x <= line.End.X; x++ {
				pointMap[fmt.Sprintf("%d,%d", x, line.Start.Y)] += 1
			}
			for x := line.End.X; x <= line.Start.X; x++ {
				pointMap[fmt.Sprintf("%d,%d", x, line.Start.Y)] += 1
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
	pointMap := make(map[string]int64)
	for _, line := range ParseInput(lines) {
		deltaX, deltaY := line.End.X-line.Start.X, line.End.Y-line.Start.Y
		// Line is guaranteed to at 45 degrees if not straight
		if deltaX != 0 {
			deltaX /= int64(math.Abs(float64(deltaX)))
		}
		if deltaY != 0 {
			deltaY /= int64(math.Abs(float64(deltaY)))
		}

		x := line.Start.X
		y := line.Start.Y
		pointMap[fmt.Sprintf("%d,%d", x, y)] += 1
		for x != line.End.X || y != line.End.Y {
			x += deltaX
			y += deltaY
			pointMap[fmt.Sprintf("%d,%d", x, y)] += 1
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

func ParseInput(lines [][]byte) []utils.Line {
	var data []utils.Line

	for _, item := range lines {
		points := bytes.Split(item, []byte(" -> "))

		startPoint := bytes.Split(points[0], []byte(","))
		startX := utils.ParseNumber(startPoint[0])
		startY := utils.ParseNumber(startPoint[1])

		endPoint := bytes.Split(points[1], []byte(","))
		endX := utils.ParseNumber(endPoint[0])
		endY := utils.ParseNumber(endPoint[1])

		data = append(data, utils.Line{
			Start: utils.Point2D{X: startX, Y: startY},
			End:   utils.Point2D{X: endX, Y: endY},
		})
	}

	return data
}
