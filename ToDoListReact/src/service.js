import axios from 'axios';

axios.defaults.baseURL = "https://localhost:7279";

axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    console.error("Error response interceptor:", error);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get("/")    
    return result.data;
  },

  addTask: async (name) => {
    const taskData = {
      Name: name,
      IsComplete: false
    };
    
    const result = await axios.post("/", taskData);
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    const item= await axios.get(`/${id}`)
    const taskData = {
      Name: item.data.Name,
      IsComplete: isComplete
    };
    
    const result = await axios.put(`/${id}`, taskData);
    return result.data;
  },

  deleteTask: async (id) => {
    const result = await axios.delete(`/${id}`);
    return result.data;
  }
};