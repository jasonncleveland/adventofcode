package day03

import (
	"testing"
)

func TestPart1(t *testing.T) {
	bits := [][]int64{
		{0, 0, 1, 0, 0},
		{1, 1, 1, 1, 0},
		{1, 0, 1, 1, 0},
		{1, 0, 1, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 1},
		{0, 0, 1, 1, 1},
		{1, 1, 1, 0, 0},
		{1, 0, 0, 0, 0},
		{1, 1, 0, 0, 1},
		{0, 0, 0, 1, 0},
		{0, 1, 0, 1, 0},
	}
	result := Part1(bits)

	expected := int64(198)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	bits := [][]int64{
		{0, 0, 1, 0, 0},
		{1, 1, 1, 1, 0},
		{1, 0, 1, 1, 0},
		{1, 0, 1, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 1},
		{0, 0, 1, 1, 1},
		{1, 1, 1, 0, 0},
		{1, 0, 0, 0, 0},
		{1, 1, 0, 0, 1},
		{0, 0, 0, 1, 0},
		{0, 1, 0, 1, 0},
	}
	result := Part2(bits)

	expected := int64(230)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}
