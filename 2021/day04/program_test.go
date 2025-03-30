package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		{
			55, 44, 52, 44, 57, 44, 53, 44, 49, 49, 44, 49, 55, 44, 50, 51, 44,
			50, 44, 48, 44, 49, 52, 44, 50, 49, 44, 50, 52, 44, 49, 48, 44,
			49, 54, 44, 49, 51, 44, 54, 44, 49, 53, 44, 50, 53, 44, 49, 50, 44,
			50, 50, 44, 49, 56, 44, 50, 48, 44, 56, 44, 49, 57, 44, 51, 44,
			50, 54, 44, 49,
		},
		{
			50, 50, 32, 49, 51, 32, 49, 55, 32, 49, 49, 32, 32, 48, 10,
			32, 56, 32, 32, 50, 32, 50, 51, 32, 32, 52, 32, 50, 52, 10,
			50, 49, 32, 32, 57, 32, 49, 52, 32, 49, 54, 32, 32, 55, 10,
			32, 54, 32, 49, 48, 32, 32, 51, 32, 49, 56, 32, 32, 53, 10,
			32, 49, 32, 49, 50, 32, 50, 48, 32, 49, 53, 32, 49, 57,
		},
		{
			32, 51, 32, 49, 53, 32, 32, 48, 32, 32, 50, 32, 50, 50, 10,
			32, 57, 32, 49, 56, 32, 49, 51, 32, 49, 55, 32, 32, 53, 10,
			49, 57, 32, 32, 56, 32, 32, 55, 32, 50, 53, 32, 50, 51, 10,
			50, 48, 32, 49, 49, 32, 49, 48, 32, 50, 52, 32, 32, 52, 10,
			49, 52, 32, 50, 49, 32, 49, 54, 32, 49, 50, 32, 32, 54,
		},
		{
			49, 52, 32, 50, 49, 32, 49, 55, 32, 50, 52, 32, 32, 52, 10,
			49, 48, 32, 49, 54, 32, 49, 53, 32, 32, 57, 32, 49, 57, 10,
			49, 56, 32, 32, 56, 32, 50, 51, 32, 50, 54, 32, 50, 48, 10,
			50, 50, 32, 49, 49, 32, 49, 51, 32, 32, 54, 32, 32, 53, 10,
			32, 50, 32, 32, 48, 32, 49, 50, 32, 32, 51, 32, 32, 55,
		},
	}

	result := Part1(lines)

	expected := int64(4512)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		{
			55, 44, 52, 44, 57, 44, 53, 44, 49, 49, 44, 49, 55, 44, 50, 51, 44,
			50, 44, 48, 44, 49, 52, 44, 50, 49, 44, 50, 52, 44, 49, 48, 44,
			49, 54, 44, 49, 51, 44, 54, 44, 49, 53, 44, 50, 53, 44, 49, 50, 44,
			50, 50, 44, 49, 56, 44, 50, 48, 44, 56, 44, 49, 57, 44, 51, 44,
			50, 54, 44, 49,
		},
		{
			50, 50, 32, 49, 51, 32, 49, 55, 32, 49, 49, 32, 32, 48, 10,
			32, 56, 32, 32, 50, 32, 50, 51, 32, 32, 52, 32, 50, 52, 10,
			50, 49, 32, 32, 57, 32, 49, 52, 32, 49, 54, 32, 32, 55, 10,
			32, 54, 32, 49, 48, 32, 32, 51, 32, 49, 56, 32, 32, 53, 10,
			32, 49, 32, 49, 50, 32, 50, 48, 32, 49, 53, 32, 49, 57,
		},
		{
			32, 51, 32, 49, 53, 32, 32, 48, 32, 32, 50, 32, 50, 50, 10,
			32, 57, 32, 49, 56, 32, 49, 51, 32, 49, 55, 32, 32, 53, 10,
			49, 57, 32, 32, 56, 32, 32, 55, 32, 50, 53, 32, 50, 51, 10,
			50, 48, 32, 49, 49, 32, 49, 48, 32, 50, 52, 32, 32, 52, 10,
			49, 52, 32, 50, 49, 32, 49, 54, 32, 49, 50, 32, 32, 54,
		},
		{
			49, 52, 32, 50, 49, 32, 49, 55, 32, 50, 52, 32, 32, 52, 10,
			49, 48, 32, 49, 54, 32, 49, 53, 32, 32, 57, 32, 49, 57, 10,
			49, 56, 32, 32, 56, 32, 50, 51, 32, 50, 54, 32, 50, 48, 10,
			50, 50, 32, 49, 49, 32, 49, 51, 32, 32, 54, 32, 32, 53, 10,
			32, 50, 32, 32, 48, 32, 49, 50, 32, 32, 51, 32, 32, 55,
		},
	}

	result := Part2(lines)

	expected := int64(1924)
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
