package day11

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("5483143223"),
		[]byte("2745854711"),
		[]byte("5264556173"),
		[]byte("6141336146"),
		[]byte("6357385478"),
		[]byte("4167524645"),
		[]byte("2176841721"),
		[]byte("6882881134"),
		[]byte("4846848554"),
		[]byte("5283751526"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(1656)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("5483143223"),
		[]byte("2745854711"),
		[]byte("5264556173"),
		[]byte("6141336146"),
		[]byte("6357385478"),
		[]byte("4167524645"),
		[]byte("2176841721"),
		[]byte("6882881134"),
		[]byte("4846848554"),
		[]byte("5283751526"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(195)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}
