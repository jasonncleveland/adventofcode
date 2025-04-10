package day08

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe"),
		[]byte("edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc"),
		[]byte("fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg"),
		[]byte("fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb"),
		[]byte("aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea"),
		[]byte("fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb"),
		[]byte("dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe"),
		[]byte("bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef"),
		[]byte("egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb"),
		[]byte("gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(26)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe"),
		[]byte("edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc"),
		[]byte("fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg"),
		[]byte("fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb"),
		[]byte("aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea"),
		[]byte("fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb"),
		[]byte("dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe"),
		[]byte("bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef"),
		[]byte("egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb"),
		[]byte("gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(61229)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expected := []entry{
		{
			[][]byte{
				{97, 99, 101, 100, 103, 102, 98},
				{99, 100, 102, 98, 101},
				{103, 99, 100, 102, 97},
				{102, 98, 99, 97, 100},
				{100, 97, 98},
				{99, 101, 102, 97, 98, 100},
				{99, 100, 102, 103, 101, 98},
				{101, 97, 102, 98},
				{99, 97, 103, 101, 100, 98},
				{97, 98},
			},
			[][]byte{
				{99, 100, 102, 101, 98},
				{102, 99, 97, 100, 98},
				{99, 100, 102, 101, 98},
				{99, 100, 98, 97, 102},
			},
		},
	}
	lines := [][]byte{
		[]byte("acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(expected))
	}
	for i := range result {
		if len(result[i].inputs) != len(expected[i].inputs) {
			t.Errorf("Length of result inputs does not match expected, got: %d, want: %d.\n", len(result[i].inputs), len(expected[i].inputs))
		}
		if len(result[i].outputs) != len(expected[i].outputs) {
			t.Errorf("Length of result outputs does not match expected, got: %d, want: %d.\n", len(result[i].outputs), len(expected[i].outputs))
		}

		for j := range result[i].inputs {
			if string(result[i].inputs[j]) != string(expected[i].inputs[j]) {
				t.Errorf("Input value at index %d not equal to expected, got: %v, want: %v\n", j, result[i].inputs[j], expected[i].inputs[j])
			}
		}
		for j := range result[i].outputs {
			if string(result[i].outputs[j]) != string(expected[i].outputs[j]) {
				t.Errorf("Output value at index %d not equal to expected, got: %v, want: %v\n", j, result[i].inputs[j], expected[i].inputs[j])
			}
		}
	}
}
