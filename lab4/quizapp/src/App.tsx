import React, { useState } from "react";
import QuestionCard from "./components/QuestionCard";
import { Question } from "./models/question";
import { UserAnswer } from "./models/userAnswer";
import { agent } from "./api/agent";
import { User } from "./models/user";

import { GlobalStyle, Wrapper } from "./App.styles";

type Props = {
  id: number;
  callback: (id: number) => void;
  user: User;
};
function App({ id, user, callback }: Props) {
  const [loading, setLoading] = useState(false);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [number, setNumber] = useState(0);
  const [hasAnswered, setHasAnswered] = useState(false);
  const [score, setScore] = useState(0);
  const [gameOver, setGameOver] = useState(true);
  const [totalQuestions, setTotalQuestions] = useState(0);

  const startTrivia = async () => {
    setLoading(true);
    setGameOver(false);
    const data = await agent.Quizes.find(id);
    setQuestions(data.questions);
    setTotalQuestions(data.questions.length);
    setNumber(0);
    setLoading(false);
    setScore(0);
  };
  const checkAnswer = async (
    e: React.MouseEvent<HTMLButtonElement>
    // ref: React.RefObject<HTMLButtonElement>
  ) => {
    if (gameOver) return;
    const answer: UserAnswer = {
      question_id: questions[number].id,
      answer: e.currentTarget.value,
      user_id: user!.id,
    };
    const questionAnswer = await agent.Quizes.check(answer, id);
    setHasAnswered(true);
    const target = e.target as HTMLButtonElement;
    console.log(`${answer.answer}${number + 1}`);
    const button = target.closest(
      `#${answer.answer}${number + 1}`
    ) as HTMLButtonElement;
    if (questionAnswer.correct) {
      setScore(score + 1);
      button.style.background = "linear-gradient(90deg, #56FFA4, #59BC86)";
    } else {
      button.style.background = "linear-gradient(90deg, #FF5656, #C16868)";
    }
  };
  const nextQuestion = () => {
    const wrapper = document.getElementById(
      "qcButtonWrapper"
    ) as HTMLDivElement;
    const buttons = wrapper.querySelectorAll("button");
    buttons.forEach((el) => {
      el.style.background = "linear-gradient(90deg, #56ccff, #6eafb4)";
    });
    setHasAnswered(false);
    const nextQ = number + 1;
    if (nextQ === totalQuestions) {
      setGameOver(true);
    } else {
      setNumber(nextQ);
    }
  };

  return (
    <>
      <GlobalStyle />
      <Wrapper>
        <h1>REACT QUIZ</h1>
        {gameOver && (
          <button className="start" onClick={startTrivia}>
            Start
          </button>
        )}
        {!gameOver && number + 1 === totalQuestions && hasAnswered && (
          <button className="start" onClick={() => callback(id)}>
            Go Back
          </button>
        )}
        {!gameOver && <p className="score">Score:{score}</p>}
        {loading && <p>Loading Questions...</p>}
        {!loading && !gameOver && (
          <QuestionCard
            questionNr={number + 1}
            totalQuestions={totalQuestions}
            hasAnswered={hasAnswered}
            question={questions[number].question}
            answers={questions[number].answers}
            callback={checkAnswer}
          />
        )}
        {!gameOver && hasAnswered && number + 1 < totalQuestions && (
          <button className="next" onClick={nextQuestion}>
            Next Question
          </button>
        )}
      </Wrapper>
    </>
  );
}

export default App;
