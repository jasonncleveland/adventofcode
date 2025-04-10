package day14

import (
	"fmt"
	"reflect"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("NNCB"),
		[]byte(""),
		[]byte("CH -> B"),
		[]byte("HH -> N"),
		[]byte("CB -> H"),
		[]byte("NH -> C"),
		[]byte("HB -> C"),
		[]byte("HC -> B"),
		[]byte("HN -> C"),
		[]byte("NN -> C"),
		[]byte("BH -> H"),
		[]byte("NC -> B"),
		[]byte("NB -> B"),
		[]byte("BN -> B"),
		[]byte("BB -> N"),
		[]byte("BC -> B"),
		[]byte("CC -> N"),
		[]byte("CN -> C"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(1588)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	t.Skip()
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expectedTemplate := "NNCB"
	expectedRules := map[string]byte{
		"CH": byte('B'),
		"HH": byte('N'),
		"CB": byte('H'),
		"NH": byte('C'),
		"HB": byte('C'),
		"HC": byte('B'),
		"HN": byte('C'),
		"BH": byte('H'),
		"NN": byte('C'),
		"NC": byte('B'),
		"NB": byte('B'),
		"BN": byte('B'),
		"BB": byte('N'),
		"BC": byte('B'),
		"CC": byte('N'),
		"CN": byte('C'),
	}
	lines := [][]byte{
		[]byte("NNCB"),
		[]byte(""),
		[]byte("CH -> B"),
		[]byte("HH -> N"),
		[]byte("CB -> H"),
		[]byte("NH -> C"),
		[]byte("HB -> C"),
		[]byte("HC -> B"),
		[]byte("HN -> C"),
		[]byte("NN -> C"),
		[]byte("BH -> H"),
		[]byte("NC -> B"),
		[]byte("NB -> B"),
		[]byte("BN -> B"),
		[]byte("BB -> N"),
		[]byte("BC -> B"),
		[]byte("CC -> N"),
		[]byte("CN -> C"),
	}

	template, rules := ParseInput(lines)
	fmt.Println(template, rules)
	if len(template) != len(expectedTemplate) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(template), len(expectedTemplate))
	}
	if len(rules) != len(expectedRules) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(rules), len(expectedRules))
	}
	if !reflect.DeepEqual(template, expectedTemplate) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", template, expectedTemplate)
	}
	if !reflect.DeepEqual(rules, expectedRules) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", rules, expectedRules)
	}
}
