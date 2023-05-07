import axios, { AxiosResponse } from "axios";
import { QuizMetaData } from "../models/quizMetaData";
import { Quiz } from "../models/quiz";
import { UserAnswer } from "../models/userAnswer";
import { QuestionAnswer } from "../models/questionAnswer";
import { User } from "../models/user";
import { CreateUser } from "../models/createUser";
import { CreateQuiz } from "../models/createQuiz";
axios.defaults.baseURL = process.env.REACT_APP_API_URL;

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) =>
    axios.post<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Quizes = {
  list: () => requests.get<QuizMetaData[]>(`/v54/quizzes`),
  find: (id: number) => requests.get<Quiz>(`/v54/quizzes/${id}`),
  check: (answer: UserAnswer, quizId: number) =>
    requests.post<QuestionAnswer>(`v54/quizzes/${quizId}/submit`, {
      data: answer,
    }),
  create: (quiz: CreateQuiz) => requests.post(`/v54/quizzes`, { data: quiz }),
  delete: (quizId: number) => requests.del(`/v54/quizzes/${quizId}`),
};
const Users = {
  create: (user: CreateUser) =>
    requests.post<User>(`/v54/users`, { data: user }),
  delete: (id: number) => requests.del(`v54/users/${id}`),
};
axios.defaults.headers.common["X-Access-Token"] =
  localStorage.getItem("x-access-token");

export const agent = {
  Quizes,
  Users,
};
