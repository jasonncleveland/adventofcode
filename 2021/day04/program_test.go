package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	numbers := []int64{7, 4, 9, 5, 11, 17, 23, 2, 0, 14, 21, 24, 10, 16, 13, 6, 15, 25, 12, 22, 18, 20, 8, 19, 3, 26, 1}
	cards := [][5][5]int64{
		{
			{22, 13, 17, 11, 0},
			{8, 2, 23, 4, 24},
			{21, 9, 14, 16, 7},
			{6, 10, 3, 18, 5},
			{1, 12, 20, 15, 19},
		},
		{
			{3, 15, 0, 2, 22},
			{9, 18, 13, 17, 5},
			{19, 8, 7, 25, 23},
			{20, 11, 10, 24, 4},
			{14, 21, 16, 12, 6},
		},
		{
			{14, 21, 17, 24, 4},
			{10, 16, 15, 9, 19},
			{18, 8, 23, 26, 20},
			{22, 11, 13, 6, 5},
			{2, 0, 12, 3, 7},
		},
	}

	result := Part1(numbers, cards)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(4512)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	numbers := []int64{7, 4, 9, 5, 11, 17, 23, 2, 0, 14, 21, 24, 10, 16, 13, 6, 15, 25, 12, 22, 18, 20, 8, 19, 3, 26, 1}
	cards := [][5][5]int64{
		{
			{22, 13, 17, 11, 0},
			{8, 2, 23, 4, 24},
			{21, 9, 14, 16, 7},
			{6, 10, 3, 18, 5},
			{1, 12, 20, 15, 19},
		},
		{
			{3, 15, 0, 2, 22},
			{9, 18, 13, 17, 5},
			{19, 8, 7, 25, 23},
			{20, 11, 10, 24, 4},
			{14, 21, 16, 12, 6},
		},
		{
			{14, 21, 17, 24, 4},
			{10, 16, 15, 9, 19},
			{18, 8, 23, 26, 20},
			{22, 11, 13, 6, 5},
			{2, 0, 12, 3, 7},
		},
	}

	result := Part2(numbers, cards)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	numbers := []int64{7, 4, 9, 5, 11}
	cards := [][5][5]int64{
		{
			{22, 13, 17, 11, 0},
			{8, 2, 23, 4, 24},
			{21, 9, 14, 16, 7},
			{6, 10, 3, 18, 5},
			{1, 12, 20, 15, 19},
		},
	}
	lines := [][]byte{
		{55, 44, 52, 44, 57, 44, 53, 44, 49, 49},
		{
			50, 50, 32, 49, 51, 32, 49, 55, 32, 49, 49, 32, 32, 48, 10,
			32, 56, 32, 32, 50, 32, 50, 51, 32, 32, 52, 32, 50, 52, 10,
			50, 49, 32, 32, 57, 32, 49, 52, 32, 49, 54, 32, 32, 55, 10,
			32, 54, 32, 49, 48, 32, 32, 51, 32, 49, 56, 32, 32, 53, 10,
			32, 49, 32, 49, 50, 32, 50, 48, 32, 49, 53, 32, 49, 57,
		},
	}

	foundNumbers, foundCards := ParseInput(lines)
	if len(foundNumbers) != len(numbers) || len(foundCards) != len(cards) {
		t.Fatalf("Length of numbers (%d vs %d) or cards (%d vs %d) is not equal to the expected length\n", len(foundNumbers), len(numbers), len(foundCards), len(cards))
	}
	for i := range foundNumbers {
		if foundNumbers[i] != numbers[i] {
			t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, foundNumbers[i], numbers[i])
		}
	}
	for cardId, card := range foundCards {
		for row := range card {
			for column := range card[row] {
				if foundCards[cardId][row][column] != cards[cardId][row][column] {
					t.Errorf("Number at index %d, %d, %d in result %d is not equal to expected %d\n", cardId, row, column, foundCards[cardId][row][column], cards[cardId][row][column])
				}
			}
		}
	}
}

func TestReadFileLines(t *testing.T) {
	line := []byte("#######\n\n#.....#\n\n#..#..#\n\n#.###.#\n\n#.###.#\n\n#..#..#\n\n#.....#\n\n#######")
	lines := [][]byte{
		[]byte("#######"),
		[]byte("#.....#"),
		[]byte("#..#..#"),
		[]byte("#.###.#"),
		[]byte("#.###.#"),
		[]byte("#..#..#"),
		[]byte("#.....#"),
		[]byte("#######"),
	}

	tempDir := t.TempDir()
	fileName := tempDir + "testFileName"
	err := os.WriteFile(fileName, line, 0777)
	if err != nil {
		t.Fatal("Could not write test file with name", fileName)
	}

	// Test with a valid file
	result := ReadFileLines(fileName)
	if len(result) != len(lines) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(lines))
	}
	for i := range result {
		if len(result[i]) != len(lines[i]) {
			t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result[i]), len(lines[i]))
		}
		for j := range result[i] {
			if result[i][j] != lines[i][j] {
				t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, result[i][j], lines[i][j])
			}
		}
	}

	// Test with an invalid file
	defer func() {
		if r := recover(); r == nil {
			t.Fatal("Expected to recover panic from file open error")
		}
	}()
	ReadFileLines("invalidData.txt")
}
