package day05

import (
	"testing"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(5)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(12)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expected := []utils.Line{
		{Start: utils.Point2D{X: 0, Y: 9}, End: utils.Point2D{X: 5, Y: 9}},
		{Start: utils.Point2D{X: 8, Y: 0}, End: utils.Point2D{X: 0, Y: 8}},
		{Start: utils.Point2D{X: 9, Y: 4}, End: utils.Point2D{X: 3, Y: 4}},
		{Start: utils.Point2D{X: 2, Y: 2}, End: utils.Point2D{X: 2, Y: 1}},
		{Start: utils.Point2D{X: 7, Y: 0}, End: utils.Point2D{X: 7, Y: 4}},
		{Start: utils.Point2D{X: 6, Y: 4}, End: utils.Point2D{X: 2, Y: 0}},
		{Start: utils.Point2D{X: 0, Y: 9}, End: utils.Point2D{X: 2, Y: 9}},
		{Start: utils.Point2D{X: 3, Y: 4}, End: utils.Point2D{X: 1, Y: 4}},
		{Start: utils.Point2D{X: 0, Y: 0}, End: utils.Point2D{X: 8, Y: 8}},
		{Start: utils.Point2D{X: 5, Y: 5}, End: utils.Point2D{X: 8, Y: 2}},
	}
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(expected))
	}
	for i := range result {
		if result[i] != expected[i] {
			t.Errorf("Value at index %d in result %v is not equal to expected %v\n", i, result[i], expected[i])
		}
	}
}
