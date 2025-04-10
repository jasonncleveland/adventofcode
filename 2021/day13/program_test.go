package day13

import (
	"reflect"
	"testing"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(17)
	if result != expected {
		t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	expected := `
█████
█   █
█   █
█   █
█████
`
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	result := Part2(lines)

	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expectedCoordinates := []utils.Point2D{
		{X: 6, Y: 10},
		{X: 0, Y: 14},
		{X: 9, Y: 10},
		{X: 0, Y: 3},
		{X: 10, Y: 4},
		{X: 4, Y: 11},
		{X: 6, Y: 0},
		{X: 6, Y: 12},
		{X: 4, Y: 1},
		{X: 0, Y: 13},
		{X: 10, Y: 12},
		{X: 3, Y: 4},
		{X: 3, Y: 0},
		{X: 8, Y: 4},
		{X: 1, Y: 10},
		{X: 2, Y: 14},
		{X: 8, Y: 10},
		{X: 9, Y: 0},
	}
	expectedInstructions := []utils.Point2D{
		{X: 0, Y: 7},
		{X: 5, Y: 0},
	}
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	coordinates, instructions := ParseInput(lines)
	if len(coordinates) != len(expectedCoordinates) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(coordinates), len(expectedCoordinates))
	}
	if len(instructions) != len(expectedInstructions) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(instructions), len(expectedInstructions))
	}
	if !reflect.DeepEqual(coordinates, expectedCoordinates) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", coordinates, expectedCoordinates)
	}
	if !reflect.DeepEqual(instructions, expectedInstructions) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", instructions, expectedInstructions)
	}
}
