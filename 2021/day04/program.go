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
	numbers, cards := ParseInput(lines)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(numbers, cards)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(numbers, cards)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(numbers []int64, cards [][5][5]int64) int64 {
	for _, number := range numbers {
		MarkNumber(cards, number)
		if cardId := CheckBingo(cards); cardId != -1 {
			score := ScoreCard(cards[cardId])
			fmt.Printf("Found bingo on card %d after number %d with score %d\n", cardId, number, score)
			return score * number
		}
	}
	return -1
}

func Part2(numbers []int64, cards [][5][5]int64) int64 {
	return -1
}

func MarkNumber(cards [][5][5]int64, number int64) {
	for cardId, card := range cards {
		for row := range card {
			for column := range card[row] {
				if card[row][column] == number {
					cards[cardId][row][column] = -1
				}
			}
		}
	}
}

func CheckBingo(cards [][5][5]int64) int64 {
	for cardId, card := range cards {
		// Check rows
		for row := range card {
			isBingo := true
			for column := range card {
				if card[row][column] != -1 {
					isBingo = false
					break
				}
			}
			if isBingo {
				return int64(cardId)
			}
		}

		// Check columns
		for column := range card {
			isBingo := true
			for row := range card {
				if card[row][column] != -1 {
					isBingo = false
					break
				}
			}
			if isBingo {
				return int64(cardId)
			}
		}
	}

	return -1
}

func ScoreCard(card [5][5]int64) int64 {
	score := int64(0)
	for row := range card {
		for column := range card[row] {
			if card[row][column] != -1 {
				score += card[row][column]
			}
		}
	}
	return score
}

func ParseInput(lines [][]byte) ([]int64, [][5][5]int64) {
	lineIndex := 0

	// Parse bingo numbers
	numberBytes := bytes.Split(lines[lineIndex], []byte(","))
	numbers := make([]int64, len(numberBytes))
	for i, bytes := range numberBytes {
		magnitude := int64(1)
		number := int64(0)
		for index := range bytes {
			number += int64(bytes[len(bytes)-1-index]-byte('0')) * magnitude
			magnitude *= 10
		}
		numbers[i] = number
	}
	lineIndex++

	// Parse bingo sheets
	cards := make([][5][5]int64, len(lines)-1)
	for ; lineIndex < len(lines); lineIndex++ {
		cardLines := bytes.Split(lines[lineIndex], []byte("\n"))
		var card [5][5]int64
		for row, line := range cardLines {
			for column := 1; column < len(line); column += 3 {
				number := int64(0)
				if line[column-1] != byte(32) {
					number += 10 * int64(line[column-1]-byte('0'))
				}
				number += int64(line[column] - byte('0'))
				card[row][column/3] = number
			}
		}
		cards[lineIndex-1] = card
	}

	return numbers, cards
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n\n"))

	return lines
}
