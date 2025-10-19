package day20

import (
	"bytes"
	"fmt"
	"strconv"
	"time"

	"advent-of-code-2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFile(fileName)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(lines)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(lines)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(lines []byte) int64 {
	data := ParseInput(lines)

	// The infinite pixels will all change to the value at position 0 (.) or 511 (#) in the algorithm
	var infinitePixel byte = '.'

	for range 2 {
		data = EnhanceImage(data, infinitePixel)
		if infinitePixel == '.' {
			infinitePixel = data.ImageEnhancementAlgorithm[0]
		} else {
			infinitePixel = data.ImageEnhancementAlgorithm[511]
		}
	}

	litPixels := 0
	for _, pixel := range data.Pixels {
		if pixel == '#' {
			litPixels += 1
		}
	}
	return int64(litPixels)
}

func Part2(lines []byte) int64 {
	data := ParseInput(lines)

	// The infinite pixels will all change to the value at position 0 (.) or 511 (#) in the algorithm
	var infinitePixel byte = '.'

	for range 50 {
		data = EnhanceImage(data, infinitePixel)
		if infinitePixel == '.' {
			infinitePixel = data.ImageEnhancementAlgorithm[0]
		} else {
			infinitePixel = data.ImageEnhancementAlgorithm[511]
		}
	}

	litPixels := 0
	for _, pixel := range data.Pixels {
		if pixel == '#' {
			litPixels += 1
		}
	}
	return int64(litPixels)
}

func EnhanceImage(data ImageData, infinitePixel byte) ImageData {
	next := make(map[utils.Point2D]byte)

	for y := data.Min.Y; y <= data.Max.Y; y++ {
		for x := data.Min.X; x <= data.Max.X; x++ {
			point := utils.Point2D{X: x, Y: y}
			nextValue := GetNextPixelValue(data.Pixels, data.ImageEnhancementAlgorithm, point, infinitePixel)
			next[point] = nextValue
		}
	}

	return ImageData{
		ImageEnhancementAlgorithm: data.ImageEnhancementAlgorithm,
		Pixels:                    next,
		Min:                       utils.Point2D{X: data.Min.X - 1, Y: data.Min.Y - 1},
		Max:                       utils.Point2D{X: data.Max.X + 1, Y: data.Max.Y + 1},
	}
}

func GetNextPixelValue(pixels map[utils.Point2D]byte, imageEnhancementAlgorithm []byte, point utils.Point2D, infinitePixel byte) byte {
	numberParts := []byte{
		// Top left
		GetPixelValue(pixels, utils.Point2D{X: point.X - 1, Y: point.Y - 1}, infinitePixel),
		// Top
		GetPixelValue(pixels, utils.Point2D{X: point.X, Y: point.Y - 1}, infinitePixel),
		// Top right
		GetPixelValue(pixels, utils.Point2D{X: point.X + 1, Y: point.Y - 1}, infinitePixel),
		// Left
		GetPixelValue(pixels, utils.Point2D{X: point.X - 1, Y: point.Y}, infinitePixel),
		// Current
		GetPixelValue(pixels, utils.Point2D{X: point.X, Y: point.Y}, infinitePixel),
		// Right
		GetPixelValue(pixels, utils.Point2D{X: point.X + 1, Y: point.Y}, infinitePixel),
		// Bottom left
		GetPixelValue(pixels, utils.Point2D{X: point.X - 1, Y: point.Y + 1}, infinitePixel),
		// Bottom
		GetPixelValue(pixels, utils.Point2D{X: point.X, Y: point.Y + 1}, infinitePixel),
		// Bottom right
		GetPixelValue(pixels, utils.Point2D{X: point.X + 1, Y: point.Y + 1}, infinitePixel),
	}
	converted, err := strconv.ParseInt(string(numberParts), 2, 64)
	if err != nil {
		panic(err)
	}
	return imageEnhancementAlgorithm[converted]
}

func GetPixelValue(pixels map[utils.Point2D]byte, point utils.Point2D, infinitePixel byte) byte {
	switch pixels[point] {
	case '#':
		return '1'
	case '.':
		return '0'
	default:
		if infinitePixel == '#' {
			return '1'
		} else {
			return '0'
		}
	}
}

func ParseInput(data []byte) ImageData {
	grid := make(map[utils.Point2D]byte)

	sls := bytes.Split(data, []byte("\n\n"))

	lines := bytes.Split(sls[1], []byte("\n"))

	min := utils.Point2D{X: 0, Y: 0}
	max := utils.Point2D{X: 0, Y: 0}
	for y, line := range lines {
		if y == int(min.Y) {
			min.Y -= 1
		}
		if y == int(max.Y) {
			max.Y += 1
		}

		for x, c := range line {
			if x == int(min.X) {
				min.X -= 1
			}
			if x == int(max.X) {
				max.X += 1
			}
			point := utils.Point2D{X: int64(x), Y: int64(y)}
			grid[point] = c
		}
	}

	return ImageData{
		ImageEnhancementAlgorithm: sls[0],
		Pixels:                    grid,
		Min:                       min,
		Max:                       max,
	}
}

type ImageData struct {
	ImageEnhancementAlgorithm []byte
	Pixels                    map[utils.Point2D]byte
	Min                       utils.Point2D
	Max                       utils.Point2D
}
