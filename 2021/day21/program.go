package day21

import (
	"fmt"
	"slices"
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
	players := ParseInput(lines)

	var die DeterministicDice

	for {
		for playerId, player := range players {
			var moves int64 = 0
			for range 3 {
				moves += die.Roll()
			}
			player.Position += moves

			// We need to wrap around to 1 when 10 is reached
			for player.Position > 10 {
				player.Position -= 10
			}

			player.Score += player.Position
			if player.Score >= 1000 {
				return players[(playerId+1)%2].Score * die.RollCount
			}
		}
	}
}

func Part2(lines [][]byte) int64 {
	players := ParseInput(lines)

	var p []PlayerData
	for _, player := range players {
		p = append(p, *player)
	}

	cache := map[string][]int64{}
	result := ProcessTurnRec(p, 0, cache)

	return slices.Max(result)
}

func ProcessTurnRec(players []PlayerData, currentPlayerIndex int64, cache map[string][]int64) []int64 {
	cacheKey := fmt.Sprintf(
		"[%d,%d,%d]-[%d,%d,%d]-%d",
		players[0].Id, players[0].Position, players[0].Score,
		players[1].Id, players[1].Position, players[1].Score,
		currentPlayerIndex,
	)

	if cache[cacheKey] != nil {
		return cache[cacheKey]
	}

	if players[0].Score >= 21 {
		return []int64{1, 0}
	} else if players[1].Score >= 21 {
		return []int64{0, 1}
	}

	possibleRolls := []int64{1, 2, 3}
	totals := make([]int64, 2)
	for _, d1 := range possibleRolls {
		for _, d2 := range possibleRolls {
			for _, d3 := range possibleRolls {
				// Deep copy players
				playersCopy := DeepCopyPlayers(players)

				playersCopy[currentPlayerIndex].Position += d1 + d2 + d3
				// We need to wrap around to 1 when 10 is reached
				for playersCopy[currentPlayerIndex].Position > 10 {
					playersCopy[currentPlayerIndex].Position -= 10
				}

				playersCopy[currentPlayerIndex].Score += playersCopy[currentPlayerIndex].Position

				nextPlayerIndex := (currentPlayerIndex + 1) % 2
				result := ProcessTurnRec(playersCopy, nextPlayerIndex, cache)
				totals[0] += result[0]
				totals[1] += result[1]
			}
		}
	}
	cache[cacheKey] = totals
	return totals
}

func DeepCopyPlayers(players []PlayerData) []PlayerData {
	var nextPlayers []PlayerData
	for _, p := range players {
		nextPlayers = append(nextPlayers, PlayerData{
			p.Id,
			p.Position,
			p.Score,
		})
	}
	return nextPlayers
}

func ParseInput(lines [][]byte) []*PlayerData {
	var players []*PlayerData

	for _, line := range lines {
		players = append(players, &PlayerData{
			Id:       utils.ParseNumber(line[7:8]),
			Position: utils.ParseNumber(line[28:29]),
			Score:    0,
		})
	}

	return players
}

type PlayerData struct {
	Id       int64
	Position int64
	Score    int64
}

type DeterministicDice struct {
	RollCount int64
	Value     int64
}

func (dice *DeterministicDice) Roll() int64 {
	dice.RollCount += 1
	dice.Value %= 100
	dice.Value += 1
	return dice.Value
}
