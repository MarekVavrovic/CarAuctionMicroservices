import { create } from "zustand";

//1. we create type
type State = {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    searchTerm: string;
    orderBy:string;
    filterBy:string;
    
}

//2. create action
type Actions = {
    setParams: (params: Partial<State>) => void;
    reset: () => void;
}

//.3 How do you want initialize your state?
const initialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '', 
    orderBy:"make",
    filterBy:"live"  
}

//4. create a hook
export const useParamsStore = create<State & Actions>((set) => ({
    ...initialState,

    setParams: (newParams: Partial<State>) => {
        set((state) => {
            if (newParams.pageNumber !== undefined) {
                return {...state, pageNumber: newParams.pageNumber}
            } else {
                return {...state, ...newParams, pageNumber: 1}
            }
        })
    },

    reset: () => set(initialState)
}))