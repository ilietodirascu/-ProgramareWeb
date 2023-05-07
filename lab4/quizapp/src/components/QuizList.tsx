import React, { useEffect, useState } from "react";
import { agent } from "../api/agent";
import { Quiz } from "../models/quiz";
import App from "../App";
import { User } from "../models/user";
import Login from "./Login";
import Navbar from "./Navbar";
import { QuizMetaData } from "../models/quizMetaData";
import { CreateQuiz } from "../models/createQuiz";
import { Card } from "semantic-ui-react";
import "../css/quizlist.css";

const QuizList = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [isPlaying, setIsPlaying] = useState(false);
  const [user, setUser] = useState<User>(null);

  const [quizes, setQuizes] = useState<Quiz[]>([]);
  const [quizId, setQuizId] = useState(0);

  useEffect(() => {
    const userString = localStorage.getItem("user");
    if (userString != null) {
      setUser(JSON.parse(userString));
    }
  }, []);
  useEffect(() => {
    if (user != null) {
      loadQuizes();
    }
  }, [user]);
  const clickedQuiz = (e: React.MouseEvent) => {
    const quiz = e.currentTarget as HTMLDivElement;
    setIsPlaying(true);
    setQuizId(+quiz.dataset.id!);
  };
  const handleLogOut = async () => {
    setIsLoading(true);
    for (const quiz of quizes) {
      await agent.Quizes.delete(quiz.id);
    }
    setQuizes([]);
    setIsLoading(false);
    window.location.reload();
  };

  const handleGoBack = (id: number) => {
    setIsPlaying(false);
    agent.Quizes.delete(id);
    setQuizes(quizes.filter((quiz) => quiz.id !== id));
  };
  async function getFinalQuizes(newQuizes: QuizMetaData[]) {
    const finalQuizes = [];

    for (const quiz of newQuizes) {
      const newQuiz = await agent.Quizes.find(quiz.id);
      finalQuizes.push(newQuiz);
    }

    return finalQuizes;
  }

  async function updateQuizes(newQuizes: QuizMetaData[]) {
    const finalQuizes = await getFinalQuizes(newQuizes);
    setQuizes(finalQuizes);
  }
  const sendCreateQuizesRequest = async () => {
    const quizData: CreateQuiz[] = [
      {
        title: "Capitals",
        questions: [
          {
            question: "What is the capital of France?",
            answers: ["Chisinau", "Balti", "Paris"],
            correct_answer: "Paris",
          },
          {
            question: "What is the capital of Germany?",
            answers: ["Chisinau", "Berlin", "Paris"],
            correct_answer: "Berlin",
          },
        ],
      },
      {
        title: "Moldova",
        questions: [
          {
            question: "What is the most drinked beverage in Moldova?",
            answers: ["Beer", "Water", "Wine"],
            correct_answer: "Water",
          },
          {
            question: "What is the official language in Moldova?",
            answers: ["Russian", "Romanian", "Moldovan"],
            correct_answer: "Romanian",
          },
        ],
      },
    ];
    for (const quiz of quizData) {
      await agent.Quizes.create(quiz);
    }
    await loadQuizes();
  };
  const loadQuizes = async () => {
    setIsLoading(true);
    const newQuizes = await agent.Quizes.list();
    if (newQuizes != null && newQuizes.length > 0) {
      await updateQuizes(newQuizes);
      setIsLoading(false);
    } else {
      await sendCreateQuizesRequest();
      setIsLoading(false);
    }
  };
  return user != null ? (
    <div className="page-container">
      <Navbar user={user} setUser={setUser} handleLogOut={handleLogOut} />
      {isPlaying && <App user={user} id={quizId} callback={handleGoBack} />}
      <div className="grid-container">
        {!isPlaying &&
          !isLoading &&
          quizes?.map((quiz) => (
            <div className="quiz_card_wrapper grid-item">
              <Card data-id={quiz.id} key={quiz.id} onClick={clickedQuiz}>
                <Card.Content header={`Quiz Title:${quiz.title}`} />
                <Card.Content
                  description={`Questions:${quiz.questions.length}`}
                />
              </Card>
            </div>
          ))}
      </div>
      {!isPlaying && !isLoading && quizes.length === 0 && (
        <div
          style={{
            color: "white",
            width: "100",
            margin: "0 auto",
            display: "flex",
            fontSize: "3rem",
            justifyContent: "center",
          }}
        >
          Refresh or logout to play again
        </div>
      )}
    </div>
  ) : (
    <Login setUser={setUser} />
  );
};
export default QuizList;
