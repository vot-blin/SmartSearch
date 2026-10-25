import http from './http'

export const aiApi = {
  search(query, limit = 500) {
    return http.post('/ai/search', { query, limit })
  },
}