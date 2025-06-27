import { applyMiddleware, createStore } from "redux";
import { rootReducer } from "./reducers";
import { thunk } from "redux-thunk";
import debounceFetchMiddleware from "./debounceFetchMiddleware";

const store = createStore(rootReducer, applyMiddleware(thunk, debounceFetchMiddleware));

export default store;