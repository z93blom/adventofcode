using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day18 {

    class Solution : ISolver {

        public string GetName() => "Operation Order";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var values = input.Lines()
                .Select(s => Parser.ToExpression(s, GetBinaryOperatorPrecedencePart1))
                .Select(e => Evaluator.Evaluate(e))
                .ToArray();
            
            return values.Sum();
        }


        object PartTwo(string input) {
            var values = input.Lines()
                .Select(s => Parser.ToExpression(s, GetBinaryOperatorPrecedencePart2))
                .Select(e => Evaluator.Evaluate(e))
                .ToArray();
            
            return values.Sum();
        }

        public int GetBinaryOperatorPrecedencePart1(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.PlusToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public int GetBinaryOperatorPrecedencePart2(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return 2;

                case SyntaxKind.StarToken:
                    return 1;

                default:
                    return 0;
            }
        }


        public enum SyntaxKind
        {
            NumberToken,
            PlusToken,
            StarToken,
            OpenParenthesisToken,
            CloseParenthesisToken,

            WhitespaceToken,
            BadToken,
            EndOfFileToken,
        }

        public record SyntaxToken
        {
            public int Value { get; init; }
            public SyntaxKind Kind { get; init; }
        }

        public class Lexer
        {
            private readonly string _text;
            private int _position;
            private int _start;
            private SyntaxKind _kind;
            private int _value;

            public Lexer(string text)
            {
                _text = text;
            }

            private char Current => Peek(0);

            private char Lookahead => Peek(1);

            private char Peek(int offset)
            {
                var index = _position + offset;
                if (index >= _text.Length)
                    return '\0';
                
                return _text[index];
            }

            public SyntaxToken Lex()
            {
                _start = _position;
                _kind = SyntaxKind.BadToken;
                _value = 0;

                switch(Current)
                {
                    case '\0':
                        _kind = SyntaxKind.EndOfFileToken;
                        _position++;
                        break;
                    
                    case '+':
                        _kind = SyntaxKind.PlusToken;
                        _position++;
                        break;
                    
                    case '*':
                        _kind = SyntaxKind.StarToken;
                        _position++;
                        break;

                    case '(':
                        _kind = SyntaxKind.OpenParenthesisToken;
                        _position++;
                        break;

                    case ')':
                        _kind = SyntaxKind.CloseParenthesisToken;
                        _position++;
                        break;

                    default:
                        if (char.IsDigit(Current))
                        {
                            ReadNumber();
                        }
                        else if (char.IsWhiteSpace(Current))
                        {
                            ReadWhiteSpace();
                        }
                        else
                        {
                            throw new Exception($"Unexpected input: '{Current}'");
                        }
                        break;
                }

                return new SyntaxToken {Kind = _kind, Value = _value};
            }

            private void ReadNumber()
            {
                while (char.IsDigit(Current))
                    _position++;

                var length = _position - _start;
                var text = _text.Substring(_start, length);
                _value = int.Parse(text);
                _kind = SyntaxKind.NumberToken;
            }

            private void ReadWhiteSpace()
            {
                while (char.IsWhiteSpace(Current))
                    _position++;

                var length = _position - _start;
                _kind = SyntaxKind.WhitespaceToken;          
            }
        }

        public class Parser
        {
            private readonly string _text;
            private readonly Func<SyntaxKind, int> _precedence;
            private readonly ImmutableArray<SyntaxToken> _tokens;
            private int _position;

            public Parser(string text, Func<SyntaxKind, int> precedence)
            {
                var tokens = new List<SyntaxToken>();
                var lexer = new Lexer(text);
                SyntaxToken token;
                do
                {
                    token = lexer.Lex();

                    if (token.Kind != SyntaxKind.WhitespaceToken &&
                        token.Kind != SyntaxKind.BadToken)
                    {
                        tokens.Add(token);
                    }

                } while (token.Kind != SyntaxKind.EndOfFileToken);

                _text = text;
                _precedence = precedence;
                _tokens = tokens.ToImmutableArray();
            }

            private SyntaxToken Peek(int offset)
            {
                var index = _position + offset;
                if (index >= _tokens.Length)
                    return _tokens[_tokens.Length - 1];

                return _tokens[index];
            }

            private SyntaxToken Current => Peek(0);

            private SyntaxToken NextToken()
            {
                var current = Current;
                _position++;
                return current;
            }

            private SyntaxToken MatchToken(SyntaxKind kind)
            {
                if (Current.Kind == kind)
                    return NextToken();

                throw new Exception($"Unexpected token. Expected {kind}, but found {Current.Kind}");
            } 

            public static ExpressionSyntax ToExpression(string s, Func<SyntaxKind, int> precedence)
            {
                var p = new Parser(s, precedence);
                return p.Parse();
            }

            public ExpressionSyntax Parse()
            {
                var expression = ParseExpression();
                var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
                return expression;
            }

            private ExpressionSyntax ParseExpression()
            {
                return ParseBinaryExpression();
            }

            private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
            {
                ExpressionSyntax left = ParsePrimaryExpression();

                while (true)
                {
                    var precedence = _precedence(Current.Kind);
                    if (precedence == 0 || precedence <= parentPrecedence)
                        break;

                    var operatorToken = NextToken();
                    var right = ParseBinaryExpression(precedence);
                    left = new BinaryExpressionSyntax(left, operatorToken, right);
                }

                return left;
            }

            private ExpressionSyntax ParsePrimaryExpression()
            {
                switch (Current.Kind)
                {
                    case SyntaxKind.OpenParenthesisToken:
                        return ParseParenthesizedExpression();

                    case SyntaxKind.NumberToken:
                        return ParseNumberLiteral();
                    
                    default:
                        throw new Exception($"Unexpected kind in ParsePrimaryExpression: {Current.Kind}");
                }
            }

            private ExpressionSyntax ParseParenthesizedExpression()
            {
                var left = MatchToken(SyntaxKind.OpenParenthesisToken);
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            private ExpressionSyntax ParseNumberLiteral()
            {
                var numberToken = MatchToken(SyntaxKind.NumberToken);
                return new LiteralExpressionSyntax(numberToken);
            }
        }

        public abstract class ExpressionSyntax
        {

        }

        public class BinaryExpressionSyntax : ExpressionSyntax
        {
            public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
            {
                Left = left;
                OperatorToken = operatorToken;
                Right = right;
            }

            public ExpressionSyntax Left { get; }
            public SyntaxToken OperatorToken { get; }
            public ExpressionSyntax Right { get; }
        }

        public class ParenthesizedExpressionSyntax : ExpressionSyntax
        {
            public ParenthesizedExpressionSyntax(SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
            {
                Left = left;
                Expression = expression;
                Right = right;
            }

            public SyntaxToken Left { get; }
            public ExpressionSyntax Expression { get; }
            public SyntaxToken Right { get; }
        }

        public class LiteralExpressionSyntax : ExpressionSyntax
        {
            public LiteralExpressionSyntax(SyntaxToken numberToken)
            {
                NumberToken = numberToken;
            }

            public SyntaxToken NumberToken { get; }
        }

        public static class Evaluator
        {
            public static long Evaluate(ExpressionSyntax root)
            {
                return EvaluateExpression(root);
            }

            private static long EvaluateExpression(ExpressionSyntax node)
            {
                return node switch 
                {
                    LiteralExpressionSyntax literalExpression => EvaluateLiteralExpression(literalExpression),
                    BinaryExpressionSyntax binaryExpression => EvaluateBinaryExpression(binaryExpression),
                    ParenthesizedExpressionSyntax e => EvaluateExpression(e.Expression),
                    _ => throw new NotImplementedException(),
                };
            }

            private static long EvaluateLiteralExpression(LiteralExpressionSyntax literalExpression)
            {
                return literalExpression.NumberToken.Value;
            }

            private static long EvaluateBinaryExpression(BinaryExpressionSyntax binaryExpression)
            {
                var left = EvaluateExpression(binaryExpression.Left);
                var right = EvaluateExpression(binaryExpression.Right);

                return binaryExpression.OperatorToken.Kind switch
                {
                    SyntaxKind.PlusToken => left + right,
                    SyntaxKind.StarToken => left * right,
                    _ => throw new Exception($"Unexpected operator {binaryExpression.OperatorToken.Kind}"),
                };
            }
        }
    }
}