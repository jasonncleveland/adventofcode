package day17

import (
	"bytes"
	"errors"
	"fmt"
	"math"
	"time"

	"advent-of-code-2021/utils"
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
	split := bytes.Split(bytes.Split(lines[0], []byte(": "))[1], []byte(", "))
	x := bytes.Split(split[0][2:], []byte(".."))
	y := bytes.Split(split[1][2:], []byte(".."))
	start := utils.Point2D{X: utils.ParseNumber(x[0]), Y: utils.ParseNumber(y[1])}
	end := utils.Point2D{X: utils.ParseNumber(x[1]), Y: utils.ParseNumber(y[0])}

	// Brute force solution to check every possible velocity
	var maxY int64 = math.MinInt64
	for vx := range int(end.X) + 1 {
		for vy := range end.Y * -1 {
			result, err := CheckTrajectory(utils.Point2D{X: int64(vx), Y: int64(vy)}, start, end)
			if err == nil {
				if result > maxY {
					maxY = result
				}
			}
		}
	}
	return maxY
}

func Part2(lines [][]byte) int64 {
	split := bytes.Split(bytes.Split(lines[0], []byte(": "))[1], []byte(", "))
	x := bytes.Split(split[0][2:], []byte(".."))
	y := bytes.Split(split[1][2:], []byte(".."))
	start := utils.Point2D{X: utils.ParseNumber(x[0]), Y: utils.ParseNumber(y[1])}
	end := utils.Point2D{X: utils.ParseNumber(x[1]), Y: utils.ParseNumber(y[0])}

	// Brute force solution to check every possible velocity
	var valid int64 = 0
	for vx := range int(end.X) + 1 {
		for vy := end.Y; vy < end.Y*-1; vy++ {
			_, err := CheckTrajectory(utils.Point2D{X: int64(vx), Y: int64(vy)}, start, end)
			if err == nil {
				valid++
			}
		}
	}
	return valid
}

func CheckTrajectory(velocity utils.Point2D, start utils.Point2D, end utils.Point2D) (int64, error) {
	current := utils.Point2D{X: 0, Y: 0}
	var maxY int64 = math.MinInt64
	for {
		if current.X >= start.X && current.X <= end.X && current.Y <= start.Y && current.Y >= end.Y {
			return maxY, nil
		}
		if current.X > end.X || current.Y < end.Y {
			return -1, errors.New("overshot the target")
		}
		current.X += velocity.X
		current.Y += velocity.Y
		if current.Y > maxY {
			maxY = current.Y
		}
		if velocity.X > 0 {
			velocity.X--
		}
		velocity.Y--
	}
}
