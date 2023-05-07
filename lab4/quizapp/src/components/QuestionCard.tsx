import React, { useRef } from "react";
import { Wrapper } from "../App.styles";
import { ButtonWrapper } from "./QuestionCard.styles";

type Props = {
  question: string;
  answers: string[];
  callback: (
    e: React.MouseEvent<HTMLButtonElement>
    // ref: React.RefObject<HTMLButtonElement>
  ) => Promise<void>;
  hasAnswered: boolean;
  questionNr: number;
  totalQuestions: number;
};

const QuestionCard: React.FC<Props> = ({
  question,
  answers,
  callback,
  questionNr,
  hasAnswered,
  totalQuestions,
}) => {
  const styledComponentRef = useRef<HTMLButtonElement>(null);
  return (
    <Wrapper>
      <p className="number">
        Question: {questionNr} / {totalQuestions}
      </p>
      <p>{question}</p>
      <div id="qcButtonWrapper" style={{ width: "100%" }}>
        {answers.map((answer) => (
          <ButtonWrapper key={answer}>
            <button
              id={`${answer}${questionNr}`}
              ref={styledComponentRef}
              disabled={hasAnswered}
              value={answer}
              onClick={(e) => callback(e)}
            >
              <span>{answer}</span>
            </button>
          </ButtonWrapper>
        ))}
      </div>
    </Wrapper>
  );
};
export default QuestionCard;
