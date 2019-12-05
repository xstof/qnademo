
const routes = [
  {
    path: '/',
    component: () => import('layouts/MyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/Index.vue') }
    ],
    meta: {
      requiresAuth: false
    }
  },
  {
    path: '/login',
    component: () => import('layouts/MyLayout.vue'),
    children: [
      {
        name: 'login',
        path: '',
        component: () => import('pages/Login.vue')
      }
    ],
    meta: {
      requiresAuth: false
    }
  },
  {
    path: '/create-session',
    component: () => import('layouts/MyLayout.vue'),
    children: [
      { path: '', name: 'createsession', component: () => import('pages/SessionCreate.vue') }
    ],
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/session/:sessionId',
    component: () => import('layouts/MyLayout.vue'),
    children: [
      {
        name: 'participant',
        path: '/',
        component: () => import('pages/Voting.vue'),
        meta: {
          requiresAuth: false
        }
      },
      {
        name: 'author',
        path: 'author',
        component: () => import('pages/AuthorView.vue'),
        meta: {
          requiresAuth: true
        }
      }
    ],
    meta: {
      requiresAuth: false
    }
  }
]

// Always leave this as last one
if (process.env.MODE !== 'ssr') {
  routes.push({
    path: '*',
    component: () => import('pages/Error404.vue')
  })
}

export default routes
