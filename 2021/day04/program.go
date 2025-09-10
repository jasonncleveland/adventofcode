package day04

import (
	"bytes"
	"fmt"
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
	numbers, cards := ParseInput(lines)

	for _, number := range numbers {
		MarkNumber(cards, number)
		if cardIds := CheckBingo(cards, nil); len(cardIds) > 0 {
			score := ScoreCard(cards[cardIds[0]])
			return score * number
		}
	}
	return -1
}

func Part2(lines [][]byte) int64 {
	numbers, cards := ParseInput(lines)

	var previousRound []int64
	for _, number := range numbers {
		MarkNumber(cards, number)
		if cardIds := CheckBingo(cards, nil); len(cardIds) > 0 {
			// If the number of cards with bingo equals the number of cards, we found the last card with bingo
			if len(cardIds) == len(cards) {
				occurences := make(map[int64]int)
				for _, cardId := range previousRound {
					occurences[cardId] += 1
				}
				for _, cardId := range cardIds {
					occurences[cardId] += 1
				}
				for cardId, count := range occurences {
					if count == 1 {
						// The cardId that only has 1 occurence is the latest to be found
						return ScoreCard(cards[cardId]) * number
					}
				}
			}
			previousRound = cardIds
		}
	}
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

func CheckBingo(cards [][5][5]int64, checkedCards []int64) []int64 {
	var cardIds []int64
	for cardId, card := range cards {
		foundBingo := false

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
				cardIds = append(cardIds, int64(cardId))
				foundBingo = true
				break
			}
		}
		if foundBingo {
			continue
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
				cardIds = append(cardIds, int64(cardId))
				foundBingo = true
				break
			}
		}
		if foundBingo {
			continue
		}
	}

	return cardIds
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
