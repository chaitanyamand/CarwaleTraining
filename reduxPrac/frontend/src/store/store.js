import { createStore, applyMiddleware, combineReducers } from 'redux';
import {thunk} from 'redux-thunk';
import { todoReducer } from '../reducer/todoReducer';

const rootReducer = combineReducers({
  todos: todoReducer,
});

const store = createStore(rootReducer, applyMiddleware(thunk));

export default store;
